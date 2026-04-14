using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Orchestrator.Infrastructure.Data;

/// <summary>
/// Minimal DbContext used solely for Data Protection key storage.
/// Points to the same duckhouse-ui database as UiDbContext so that
/// the orchestrator and UI server share the same key ring.
/// The DataProtectionKeys table is created by the UI server's migration.
/// </summary>
public class DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options)
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
}
