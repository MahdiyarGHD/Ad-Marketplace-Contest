using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class DealEfConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder
            .ToTable(AdMarketDbContextSchema.Deal.TableName, AdMarketDbContextSchema.DefaultSchema);
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.AmountTon)
            .IsRequired()
            .HasPrecision(18, 9);
        
        builder
            .Property(x => x.AdFormat)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.EscrowWalletAddress)
            .HasMaxLength(64);
        
        builder
            .Property(x => x.TransactionHash)
            .HasMaxLength(128);
        
        builder
            .Property(x => x.Status)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.DraftStatus)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.AdvertiserFeedback)
            .HasMaxLength(1024);

        builder
            .Property(x => x.ChannelUnitPrice)
            .IsRequired()
            .HasPrecision(18, 9);

        builder
            .Property(x => x.CreatedAt)
            .IsRequired();
        
        builder
            .HasOne(x => x.Campaign)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.CampaignId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Application)
            .WithOne(x => x.Deal)
            .HasForeignKey<Deal>(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Invitation)
            .WithOne(x => x.Deal)
            .HasForeignKey<Deal>(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.ChannelApplication)
            .WithOne(x => x.Deal)
            .HasForeignKey<Deal>(x => x.ChannelApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Advertiser)
            .WithMany()
            .HasForeignKey(x => x.AdvertiserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => x.ApplicationId)
            .IsUnique();
        
        builder
            .HasIndex(x => x.InvitationId)
            .IsUnique();
        
        builder
            .HasIndex(x => x.ChannelApplicationId)
            .IsUnique();
        
        builder
            .HasIndex(x => x.Status);
        
        builder
            .HasIndex(x => x.AutoCancelAt);
    }
}
