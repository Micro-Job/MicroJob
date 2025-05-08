using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations.ExamConfigurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(x=> x.IntroDescription).HasMaxLength(4096);

            builder.HasOne(e => e.Company)
                   .WithMany(c => c.Exams)
                   .HasForeignKey(e => e.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(e => e.ExamQuestions)
                    .WithOne(eq => eq.Exam)
                    .HasForeignKey(eq => eq.ExamId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}