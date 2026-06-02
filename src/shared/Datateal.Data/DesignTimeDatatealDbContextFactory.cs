using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datateal.Data;

public class DesignTimeDatatealDbContextFactory : IDesignTimeDbContextFactory<DatatealDbContext>
{
    public DatatealDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatatealDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=datateal-ui;Username=postgres");
        return new DatatealDbContext(optionsBuilder.Options);
    }
}
