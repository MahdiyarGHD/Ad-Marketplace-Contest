using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class UserEfConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.User.TableName);

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.UserId)
            .IsUnique();
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.LastName)
            .IsRequired(false)
            .HasMaxLength(64);

        builder.Property(x => x.Balance)
            .HasDefaultValue(0)
            .HasPrecision(32, 8);
        
        builder.Property(x => x.UserName)
            .IsRequired(false)
            .HasMaxLength(64);
    }
}