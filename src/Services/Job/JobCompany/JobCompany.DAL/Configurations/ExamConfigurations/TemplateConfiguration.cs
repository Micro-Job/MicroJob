using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.Infrastructure.Configurations.ExamConfigurations
{
    public class TemplateConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.ViewCount)
                   .HasDefaultValue(0);

            builder.HasMany(t => t.Exams)
                   .WithOne(e => e.Template)
                   .HasForeignKey(e => e.TemplateId)
                   .OnDelete(DeleteBehavior.SetNull); // Template silinərsə, bağlı Exams NULL olacaq
        }
    }
}