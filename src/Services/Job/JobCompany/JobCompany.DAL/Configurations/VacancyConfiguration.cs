﻿using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
    {
        public void Configure(EntityTypeBuilder<Vacancy> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(v => v.CompanyName)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(v => v.Title)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(v => v.CompanyLogo)
                   .HasMaxLength(255);

            builder.Property(v => v.Requirement)
                   .IsRequired();           
            
            builder.Property(v => v.Email)
                   .HasMaxLength(32);

            builder.Property(v => v.StartDate)
                   .IsRequired();

            builder.Property(v => v.Location)
                   .HasMaxLength(128);

            builder.Property(v => v.MainSalary)
                   .HasColumnType("decimal(18,2)");

            builder.Property(v => v.MaxSalary)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Location)
                   .HasMaxLength(128);

            builder.Property(v => v.Description)
                   .HasMaxLength(1024)
                   .IsRequired();

            builder.Property(v => v.Requirement)
                   .IsRequired()
                   .HasMaxLength(1024);

            builder.HasOne(v => v.Country)
                   .WithMany(c => c.Vacancies)
                   .HasForeignKey(v => v.CountryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.City)
                   .WithMany()
                   .HasForeignKey(v => v.CityId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Category)
                   .WithMany()
                   .HasForeignKey(v => v.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.VacancyTest)
                   .WithMany()
                   .HasForeignKey(v => v.VacancyTestId)
                   .OnDelete(DeleteBehavior.SetNull);

        }
    }
}