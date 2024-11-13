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

            builder.Property(c => c.CategoryName)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.HasMany(c => c.Vacancies)
                   .WithOne(v => v.Category)
                   .HasForeignKey(v => v.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}