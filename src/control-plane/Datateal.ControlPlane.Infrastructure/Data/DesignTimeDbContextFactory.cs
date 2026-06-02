using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datateal.ControlPlane.Infrastructure.Data;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ControlPlaneDbContext>
{
    public ControlPlaneDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ControlPlaneDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=datateal-control-plane;Username=postgres");
        return new ControlPlaneDbContext(optionsBuilder.Options);
    }
}
