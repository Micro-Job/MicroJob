using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.HasMany(c => c.Cities)
                   .WithOne(city => city.Country)
                   .HasForeignKey(city => city.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Vacancies)
                   .WithOne(v => v.Country)
                   .HasForeignKey(v => v.CountryId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.Companies)
                   .WithOne(v => v.Country)
                   .HasForeignKey(v => v.CountryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}