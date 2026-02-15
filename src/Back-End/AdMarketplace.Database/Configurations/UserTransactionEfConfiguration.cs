using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class UserTransactionEfConfiguration : IEntityTypeConfiguration<UserTransaction>
{
    public void Configure(EntityTypeBuilder<UserTransaction> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.UserTransaction.TableName, AdMarketDbContextSchema.DefaultSchema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.TransactionHash)
            .IsUnique();

        builder.Property(x => x.TransactionHash)
            .IsRequired()
            .HasMaxLength(64); 

        builder.Property(x => x.LogicalTime)
            .IsRequired();

        builder.Property(x => x.Destination)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Amount)
            .HasPrecision(20, 9); 

        builder.HasOne(x => x.User)
            .WithMany() 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}