using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class ChannelPricingEfConfiguration : IEntityTypeConfiguration<ChannelPricing>
{
    public void Configure(EntityTypeBuilder<ChannelPricing> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.ChannelPricing.TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ChannelId)
            .IsRequired();

        builder.Property(x => x.AdFormat)
            .IsRequired();

        builder.Property(x => x.PriceType)
            .IsRequired();

        builder.Property(x => x.PriceTon)
            .IsRequired()
            .HasPrecision(18, 9);

        builder.HasOne(x => x.Channel)
            .WithMany(x => x.Pricings)
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

