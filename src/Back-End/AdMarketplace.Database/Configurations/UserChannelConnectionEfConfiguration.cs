using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class UserChannelConnectionEfConfiguration : IEntityTypeConfiguration<UserChannelConnection>
{
    public void Configure(EntityTypeBuilder<UserChannelConnection> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.UserChannelLink.TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ChatId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.ChatId })
            .IsUnique();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

