using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Repositories;
using DuckHouse.Orchestrator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Orchestrator.Infrastructure.Repositories;

internal class JobRepository(OrchestratorDbContext db) : IJobRepository
{
    public async Task<IReadOnlyList<Job>> GetJobsAsync(CancellationToken cancellationToken = default) =>
        await db.Jobs
            .Include(j => j.Parameters)
            .Include(j => j.Schedules)
            .OrderBy(j => j.Name)
            .ToListAsync(cancellationToken);

    public async Task<Job?> GetJobAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Jobs
            .Include(j => j.Parameters)
            .Include(j => j.Tasks).ThenInclude(t => t.Dependencies)
            .Include(j => j.Schedules)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

    public async Task<Job?> GetJobByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await db.Jobs
            .Include(j => j.Parameters)
            .Include(j => j.Tasks).ThenInclude(t => t.Dependencies)
            .Include(j => j.Schedules)
            .FirstOrDefaultAsync(j => j.Name == name, cancellationToken);

    public async Task<Job> CreateJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        job.Id = Guid.CreateVersion7();
        job.CreatedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        foreach (var p in job.Parameters) p.Id = Guid.CreateVersion7();
        foreach (var t in job.Tasks)
        {
            t.Id = Guid.CreateVersion7();
            foreach (var d in t.Dependencies) d.Id = Guid.CreateVersion7();
        }
        foreach (var s in job.Schedules) s.Id = Guid.CreateVersion7();

        db.Jobs.Add(job);
        await db.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task<Job?> UpdateJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        var existing = await db.Jobs.FindAsync([job.Id], cancellationToken);
        if (existing is null) return null;

        existing.Name = job.Name;
        existing.Description = job.Description;
        existing.FolderId = job.FolderId;
        existing.MaxConcurrentRuns = job.MaxConcurrentRuns;
        existing.IsEnabled = job.IsEnabled;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteJobAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await db.Jobs.FindAsync([id], cancellationToken);
        if (job is null) return false;
        db.Jobs.Remove(job);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
