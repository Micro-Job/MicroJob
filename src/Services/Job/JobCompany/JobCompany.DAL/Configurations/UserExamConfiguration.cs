using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class UserExamConfiguration : IEntityTypeConfiguration<UserExam>
{
    public void Configure(EntityTypeBuilder<UserExam> builder)
    {
        builder.HasOne(ua => ua.Exam)
               .WithMany(u => u.UserExams)
               .HasForeignKey(ua => ua.ExamId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
