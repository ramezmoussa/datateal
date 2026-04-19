using System.Collections.Concurrent;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using DuckHouse.Core.Nodes;
using DuckHouse.Orchestrator.Application.Validation;
using DuckHouse.Orchestrator.Core.Interfaces;
using DuckHouse.Orchestrator.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace DuckHouse.Orchestrator.Application.Engine;

/// <summary>
/// Manages node provisioning and cleanup for a single job run.
/// Each NodePoolRef maps to one shared node; multiple tasks can share it.
/// </summary>
public class NodeManager(
    IControlPlaneClient controlPlane,
    INodePoolConfigRepository nodePoolConfigRepo,
    IWheelPackageReader wheelPackageReader,
    IEnvironmentResolver environmentResolver,
    Guid jobRunId,
    ILogger logger)
{
    private readonly ConcurrentDictionary<string, NodeAllocation> _allocations = new();
    private static readonly TimeSpan NodeReadyTimeout = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan NodePollInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan KernelCreateRetryInterval = TimeSpan.FromSeconds(5);
    private const int KernelCreateMaxRetries = 6;

    private record NodeAllocation(string NodeName, bool Provisioned);

    /// <summary>
    /// Ensures a node is provisioned and running for the given NodePoolRef.
    /// Returns the node name. Provisions on first call, returns cached on subsequent calls.
    /// </summary>
    public async Task<string> EnsureNodeAsync(string nodePoolRef, CancellationToken ct)
    {
        if (_allocations.TryGetValue(nodePoolRef, out var existing))
            return existing.NodeName;

        var config = await nodePoolConfigRepo.GetByNameAsync(nodePoolRef, ct)
            ?? throw new InvalidOperationException(
                $"Node pool configuration '{nodePoolRef}' not found.");

        var nameError = NodeNameValidator.ValidateNodePoolName(nodePoolRef);
        if (nameError is not null)
            throw new InvalidOperationException(
                $"Node pool name '{nodePoolRef}' is not a valid AKS node pool name: {nameError}");

        // Generate an AKS-valid node name: 'j' + 11 lowercase hex chars from the SHA-256
        // hash of (runId, poolRef). Always exactly 12 characters, starts with a letter,
        // and unique per (run, pool) pair while still being identifiable as job-run generated.
        var hashInput = SHA256.HashData(Encoding.UTF8.GetBytes($"{jobRunId:N}{nodePoolRef}"));
        var nodeName = "j" + Convert.ToHexString(hashInput)[..11].ToLowerInvariant();
        logger.LogInformation("Provisioning node '{NodeName}' for pool '{PoolRef}' (VM: {VmSize})",
            nodeName, nodePoolRef, config.VmSize);

        IReadOnlyList<WheelContent>? wheelContents = null;
        if (config.WheelPackageIds is { Count: > 0 })
        {
            wheelContents = await wheelPackageReader.GetWheelContentsAsync(config.WheelPackageIds, ct);
        }

        var resolved = await environmentResolver.ResolveAsync(
            config.EnvironmentVariableIds, config.SecretIds, ct);

        await controlPlane.CreateNodeAsync(
            nodeName,
            config.VmSize,
            config.KernelIdleTimeout,
            config.NodeIdleTimeout,
            config.KernelRequirements,
            wheelContents,
            resolved.Variables.Count > 0 ? resolved.Variables : null,
            resolved.Secrets.Count > 0 ? resolved.Secrets : null,
            ct);

        // Poll until node is running
        var deadline = DateTime.UtcNow + NodeReadyTimeout;
        while (DateTime.UtcNow < deadline)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(NodePollInterval, ct);

            var node = await controlPlane.GetNodeAsync(nodeName, ct);
            if (node is null) continue;

            if (node.State == NodeState.Running)
            {
                logger.LogInformation("Node '{NodeName}' is running", nodeName);
                _allocations[nodePoolRef] = new NodeAllocation(nodeName, true);
                return nodeName;
            }

            if (node.State == NodeState.Failure)
                throw new InvalidOperationException(
                    $"Node '{nodeName}' failed to provision.");
        }

        throw new TimeoutException(
            $"Node '{nodeName}' did not become ready within {NodeReadyTimeout.TotalMinutes} minutes.");
    }

    /// <summary>
    /// Creates a kernel on the node associated with the given NodePoolRef.
    /// Retries on transient runtime errors — on AKS the pod may not be ready
    /// immediately after the node pool reports Running.
    /// </summary>
    public async Task<(string nodeName, string kernelId)> CreateKernelAsync(
        string nodePoolRef, CancellationToken ct)
    {
        var nodeName = await EnsureNodeAsync(nodePoolRef, ct);

        HttpRequestException? lastEx = null;
        for (var attempt = 0; attempt <= KernelCreateMaxRetries; attempt++)
        {
            if (attempt > 0)
            {
                logger.LogWarning(
                    "Kernel creation on node '{NodeName}' failed with {StatusCode} — runtime may not be ready yet. " +
                    "Retrying in {Interval}s (attempt {Attempt}/{Max})",
                    nodeName, (int?)lastEx!.StatusCode, KernelCreateRetryInterval.TotalSeconds,
                    attempt, KernelCreateMaxRetries);
                await Task.Delay(KernelCreateRetryInterval, ct);
            }

            try
            {
                var kernel = await controlPlane.CreateKernelAsync(nodeName, ct);
                logger.LogInformation("Created kernel '{KernelId}' on node '{NodeName}'",
                    kernel.Id, nodeName);
                return (nodeName, kernel.Id);
            }
            catch (HttpRequestException ex) when (IsTransientRuntimeError(ex))
            {
                lastEx = ex;
            }
        }

        throw lastEx!;
    }

    private static bool IsTransientRuntimeError(HttpRequestException ex) =>
        ex.StatusCode is
            System.Net.HttpStatusCode.BadRequest            // 400 — runtime not fully initialised
            or System.Net.HttpStatusCode.InternalServerError // 500 — unexpected startup error
            or System.Net.HttpStatusCode.BadGateway          // 502 — pod not yet reachable
            or System.Net.HttpStatusCode.ServiceUnavailable  // 503 — pod not ready
            or System.Net.HttpStatusCode.GatewayTimeout;     // 504 — pod responded too slowly

    /// <summary>
    /// Deletes a specific kernel.
    /// </summary>
    public async Task CleanupKernelAsync(string nodeName, string kernelId, CancellationToken ct)
    {
        try
        {
            await controlPlane.DeleteKernelAsync(nodeName, kernelId, ct);
            logger.LogInformation("Deleted kernel '{KernelId}' on node '{NodeName}'",
                kernelId, nodeName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete kernel '{KernelId}' on node '{NodeName}'",
                kernelId, nodeName);
        }
    }

    /// <summary>
    /// Cleans up all nodes provisioned for this job run.
    /// </summary>
    public async Task CleanupAllAsync(CancellationToken ct)
    {
        foreach (var (poolRef, alloc) in _allocations)
        {
            if (!alloc.Provisioned) continue;
            try
            {
                logger.LogInformation("Deleting node '{NodeName}' (pool '{PoolRef}')",
                    alloc.NodeName, poolRef);
                await controlPlane.DeleteNodeAsync(alloc.NodeName, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete node '{NodeName}'", alloc.NodeName);
            }
        }
        _allocations.Clear();
    }
}
