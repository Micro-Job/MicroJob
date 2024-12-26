using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.Infrastructure.Configurations.ExamConfigurations
{
       public class QuestionConfiguration : IEntityTypeConfiguration<Question>
       {
              public void Configure(EntityTypeBuilder<Question> builder)
              {
                     builder.HasKey(q => q.Id);

                     builder.Property(q => q.Title)
                            .IsRequired()
                            .HasMaxLength(64);

                     builder.Property(q => q.QuestionType)
                            .IsRequired();

                     builder.Property(q => q.IsRequired)
                            .IsRequired();

                     builder.HasOne(q => q.Exam)
                            .WithMany(e => e.Questions)
                            .HasForeignKey(q => q.ExamId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasMany(q => q.Answers)
                            .WithOne(a => a.Question)
                            .HasForeignKey(a => a.QuestionId)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}