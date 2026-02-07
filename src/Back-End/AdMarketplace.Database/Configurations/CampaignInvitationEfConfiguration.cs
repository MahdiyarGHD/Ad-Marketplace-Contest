using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class CampaignInvitationEfConfiguration : IEntityTypeConfiguration<CampaignInvitation>
{
    public void Configure(EntityTypeBuilder<CampaignInvitation> builder)
    {
        builder
            .ToTable(AdMarketDbContextSchema.CampaignInvitation.TableName, AdMarketDbContextSchema.DefaultSchema);
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.ProposedAdFormat)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.ProposedPriceType)
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
            .HasOne(x => x.Campaign)
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasIndex(x => new { x.CampaignId, x.ChannelId })
            .IsUnique();
    }
}
