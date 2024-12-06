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

            builder.Property(e => e.LogoUrl)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasOne(e => e.Template)
                   .WithMany()
                   .HasForeignKey(e => e.TemplateId)
                   .OnDelete(DeleteBehavior.SetNull); // Template silinərsə, Exam üçün NULL qoyulacaq

            builder.HasMany(e => e.Questions)
                   .WithOne(q => q.Exam)
                   .HasForeignKey(q => q.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}