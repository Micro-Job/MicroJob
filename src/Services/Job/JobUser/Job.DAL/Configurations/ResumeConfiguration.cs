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
            builder.Property(x => x.FirstName).HasMaxLength(32);
            builder.Property(x => x.LastName).HasMaxLength(32);

            builder.HasMany(r => r.Educations)
                   .WithOne(e => e.Resume)
                   .HasForeignKey(e => e.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Experiences)
                   .WithOne(ex => ex.Resume)
                   .HasForeignKey(ex => ex.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.PhoneNumbers)
                    .WithOne(ei => ei.Resume)
                    .HasForeignKey(ei => ei.ResumeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Languages)
                    .WithOne(ei => ei.Resume)
                    .HasForeignKey(ei => ei.ResumeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Certificates)
                    .WithOne(ei => ei.Resume)
                    .HasForeignKey(ei => ei.ResumeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ResumeSkills)
                    .WithOne(rs => rs.Resume)
                    .HasForeignKey(rs => rs.ResumeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                   .WithOne(u => u.Resume)
                   .HasForeignKey<Resume>(r => r.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.SavedResumes)
                    .WithOne(x => x.Resume)
                    .HasForeignKey(x => x.ResumeId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Position)
                   .WithMany(p => p.Resumes)
                   .HasForeignKey(r => r.PositionId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}