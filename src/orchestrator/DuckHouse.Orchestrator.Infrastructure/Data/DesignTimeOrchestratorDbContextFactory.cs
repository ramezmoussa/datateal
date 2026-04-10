using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DuckHouse.Orchestrator.Infrastructure.Data;

public class DesignTimeOrchestratorDbContextFactory : IDesignTimeDbContextFactory<OrchestratorDbContext>
{
    public OrchestratorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrchestratorDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=duckhouse-ui;Username=postgres");
        return new OrchestratorDbContext(optionsBuilder.Options);
    }
}
