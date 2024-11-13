﻿using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(c => c.CountryName)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.HasMany(c => c.Cities)
                   .WithOne()
                   .HasForeignKey(city => city.CountryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Vacancies)
                   .WithOne(v => v.Country)
                   .HasForeignKey(v => v.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}