using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Port> Ports => Set<Port>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Container> Containers => Set<Container>();
    public DbSet<ContainerEvent> ContainerEvents => Set<ContainerEvent>();
    public DbSet<ContainerType> ContainerTypes => Set<ContainerType>();

    public DbSet<TariffTier> TariffTiers => Set<TariffTier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}