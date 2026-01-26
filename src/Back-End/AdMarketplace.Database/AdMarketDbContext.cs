using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Database;

public class AdMarketDbContext(DbContextOptions<AdMarketDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Agent> Agents => Set<Agent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(AdMarketDbContextSchema.DefaultSchema);
        
        var assembly = typeof(AdMarketDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}