using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder.HasOne(x => x.Resume)
                .WithMany(x => x.Experiences)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.OrganizationName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PositionName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PositionDescription)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired(false);

            builder.Property(x => x.IsCurrentOrganization)
                .IsRequired();
        }
    }
}