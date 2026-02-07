using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class CampaignApplicationEfConfiguration : IEntityTypeConfiguration<CampaignApplication>
{
    public void Configure(EntityTypeBuilder<CampaignApplication> builder)
    {
        builder
            .ToTable(AdMarketDbContextSchema.CampaignApplication.TableName, AdMarketDbContextSchema.DefaultSchema);
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.ProposedAdFormat)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.ProposedPriceTon)
            .IsRequired()
            .HasPrecision(18, 9);
        
        builder
            .Property(x => x.Message)
            .HasMaxLength(1024);
        
        builder
            .Property(x => x.Status)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.RejectionReason)
            .HasMaxLength(512);
        
        builder
            .Property(x => x.CreatedAt)
            .IsRequired();
        
        builder
            .HasOne(x => x.Campaign)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => new { x.CampaignId, x.ChannelId })
            .IsUnique();
    }
}
