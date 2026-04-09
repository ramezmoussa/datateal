using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DuckHouse.Ui.Server.Infrastructure.Data;

internal class DesignTimeUiDbContextFactory : IDesignTimeDbContextFactory<UiDbContext>
{
    public UiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UiDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=duckhouse-ui;Username=postgres");
        return new UiDbContext(optionsBuilder.Options);
    }
}
