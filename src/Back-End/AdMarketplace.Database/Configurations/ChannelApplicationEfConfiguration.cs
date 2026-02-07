using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class ChannelApplicationEfConfiguration : IEntityTypeConfiguration<ChannelApplication>
{
    public void Configure(EntityTypeBuilder<ChannelApplication> builder)
    {
        builder
            .ToTable(AdMarketDbContextSchema.ChannelApplication.TableName, AdMarketDbContextSchema.DefaultSchema);
        
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
            .Property(x => x.CreatedAt)
            .IsRequired();
        
        builder
            .HasOne(x => x.Channel)
            .WithMany(x => x.ChannelApplications)
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Advertiser)
            .WithMany()
            .HasForeignKey(x => x.AdvertiserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => new { x.ChannelId, x.AdvertiserId })
            .IsUnique();
    }
}
