using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(c => c.Country)
                   .WithMany(country => country.Cities)
                   .HasForeignKey(c => c.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Vacancies)
                   .WithOne(v => v.City)
                   .HasForeignKey(v => v.CityId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(c => c.Companies)
                   .WithOne(v => v.City)
                   .HasForeignKey(v => v.CityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}