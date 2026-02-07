using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Database;

public class AdMarketDbContext(DbContextOptions<AdMarketDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UserChannelConnection> UserChannelConnection => Set<UserChannelConnection>();
    public DbSet<ChannelPricing> ChannelPricings => Set<ChannelPricing>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignApplication> CampaignApplications => Set<CampaignApplication>();
    public DbSet<CampaignInvitation> CampaignInvitations => Set<CampaignInvitation>();
    public DbSet<ChannelApplication> ChannelApplications => Set<ChannelApplication>();
    public DbSet<Deal> Deals => Set<Deal>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(AdMarketDbContextSchema.DefaultSchema);
        
        var assembly = typeof(AdMarketDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}