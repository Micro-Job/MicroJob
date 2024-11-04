using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            builder.HasOne(x => x.Resume)
                .WithMany(x => x.Educations)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.InstitutionName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Profession)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired(false);

            builder.Property(x => x.IsCurrentEducation)
                .IsRequired();

            builder.Property(x => x.ProfessionDegree)
                .IsRequired();
        }
    }
}