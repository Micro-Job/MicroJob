using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(c => c.Vacancies)
                   .WithOne(v => v.Category)
                   .HasForeignKey(v => v.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Companies)
                   .WithOne(v => v.Category)
                   .HasForeignKey(v => v.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Translations)
                   .WithOne(t => t.Category)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}