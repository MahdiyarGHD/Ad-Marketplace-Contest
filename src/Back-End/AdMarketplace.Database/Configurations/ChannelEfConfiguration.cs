using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class ChannelEfConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.Channel.TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.ChatId)
            .IsUnique();

        builder.Property(x => x.ChatId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Username)
            .IsRequired(false)
            .HasMaxLength(64);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(1024);

        builder.Property(x => x.SubscriberCount)
            .IsRequired();

        builder.Property(x => x.PremiumCount)
            .IsRequired();

        builder.Property(x => x.AverageViews)
            .IsRequired();

        builder.OwnsMany(x => x.LanguageDistributionJson, navigationBuilder =>
        {
            navigationBuilder.ToJson();
            navigationBuilder.Property(x => x.Language)
                .IsRequired()
                .HasMaxLength(16);
            navigationBuilder.Property(x => x.Percentage)
                .IsRequired();
        });

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(ChannelStatusType.UnReady);
        
        builder.Property(x => x.OwnerId)
            .IsRequired();

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CategoryId)
            .IsRequired(false);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(x => x.LastReadinessCheckAt)
            .IsRequired(false);

        builder.Property(x => x.AgentId)
            .IsRequired(false);

        builder.HasOne(x => x.Agent)
            .WithMany()
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
