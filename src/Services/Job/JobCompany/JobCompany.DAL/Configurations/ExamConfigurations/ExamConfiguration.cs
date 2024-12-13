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
            builder.HasMany(e => e.Questions)
                   .WithOne(q => q.Exam)
                   .HasForeignKey(q => q.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Company)
                   .WithMany(c => c.Exams)
                   .HasForeignKey(e => e.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}