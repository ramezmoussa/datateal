namespace DuckHouse.Core.Kernels;

public record KernelInfo(string Id, string Status, DateTimeOffset CreatedAt, DateTimeOffset LastActivity);
