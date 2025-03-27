using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.ParentPositionId)
            .IsRequired(false);

        builder.HasMany(p => p.SubPositions)
            .WithOne(p => p.ParentPosition)
            .HasForeignKey(p => p.ParentPositionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

