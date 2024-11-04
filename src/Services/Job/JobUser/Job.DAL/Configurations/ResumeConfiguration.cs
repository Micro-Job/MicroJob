using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.HasOne(r => r.Person)
                   .WithOne()
                   .HasForeignKey<Resume>(r => r.PersonId)
                   .IsRequired();

            builder.HasMany(r => r.Educations)
                   .WithOne(e => e.Resume)
                   .HasForeignKey(e => e.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Experiences)
                   .WithOne(ex => ex.Resume)
                   .HasForeignKey(ex => ex.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ExtraInformations)
                   .WithOne(ei => ei.Resume)
                   .HasForeignKey(ei => ei.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}