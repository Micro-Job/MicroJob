using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.Property(r => r.FatherName).HasMaxLength(32).IsRequired();
            builder.Property(r => r.UserPhoto).HasMaxLength(255);
            builder.Property(r => r.Adress).HasMaxLength(128);

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

            builder.HasMany(r => r.PhoneNumbers)
                    .WithOne(ei => ei.Resume)
                    .HasForeignKey(ei => ei.ResumeId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}