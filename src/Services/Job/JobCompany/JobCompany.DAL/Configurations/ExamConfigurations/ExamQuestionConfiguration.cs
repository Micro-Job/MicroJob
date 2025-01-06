using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations.ExamConfigurations
{
    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder.HasKey(eq => eq.Id);

            builder.HasOne(eq => eq.Exam)
                   .WithMany(e => e.ExamQuestions)
                   .HasForeignKey(eq => eq.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(eq => eq.Question)
                   .WithMany(q => q.ExamQuestions)
                   .HasForeignKey(eq => eq.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
