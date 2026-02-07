using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class CampaignEfConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder
            .ToTable(AdMarketDbContextSchema.Campaign.TableName, AdMarketDbContextSchema.DefaultSchema);
        
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(256);
        
        builder
            .Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2048);
        
        builder
            .Property(x => x.Brief)
            .HasMaxLength(4096);
        
        builder
            .Property(x => x.BudgetTon)
            .IsRequired()
            .HasPrecision(18, 9);
        
        builder
            .Property(x => x.MaxPricePerPlacement)
            .HasPrecision(18, 9);
        
        builder
            .Property(x => x.Status)
            .IsRequired()
            .HasConversion<byte>();
        
        builder
            .Property(x => x.CreatedAt)
            .IsRequired();
        
        builder
            .OwnsOne(x => x.TargetingJson, targetingBuilder =>
            {
                targetingBuilder.ToJson();
                targetingBuilder.Property(x => x.MinSubscribers);
                targetingBuilder.Property(x => x.MaxSubscribers);
                targetingBuilder.Property(x => x.MinAverageViews);
                targetingBuilder.Property(x => x.MinPremiumCount);
                targetingBuilder.Property(x => x.PreferredCategoryIds);
                targetingBuilder.Property(x => x.PreferredLanguages);
                targetingBuilder.Property(x => x.PreferredAdFormats);
                targetingBuilder.Property(x => x.PreferredPriceTypes);
            });

        builder
            .OwnsOne(x => x.CreativeJson, creativeBuilder =>
            {
                creativeBuilder.ToJson();
                creativeBuilder.Property(x => x.RequiresApproval);
                creativeBuilder.OwnsMany(x => x.CallToActionButtons, buttonBuilder =>
                {
                    buttonBuilder.Property(x => x.Text).HasMaxLength(64);
                    buttonBuilder.Property(x => x.Url).HasMaxLength(512);
                });
            });
        
        builder
            .HasOne(x => x.Advertiser)
            .WithMany()
            .HasForeignKey(x => x.AdvertiserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder
            .HasMany(x => x.Applications)
            .WithOne(x => x.Campaign)
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(x => x.Deals)
            .WithOne(x => x.Campaign)
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
