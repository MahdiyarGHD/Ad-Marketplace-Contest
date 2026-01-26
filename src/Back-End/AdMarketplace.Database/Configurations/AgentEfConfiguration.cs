using AdMarketplace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdMarketplace.Database.Configurations;

public class AgentEfConfiguration: IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable(AdMarketDbContextSchema.Agent.TableName);

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
        
        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(x => x.UserName)
            .IsRequired(false)
            .HasMaxLength(64);
        
        builder.Property(x => x.SessionPath)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.Property(x => x.SessionPath)
            .IsRequired()
            .HasDefaultValue(true);
    }
}