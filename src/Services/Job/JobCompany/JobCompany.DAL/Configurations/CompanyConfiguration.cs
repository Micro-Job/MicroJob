﻿using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.CompanyInformation)
                .HasMaxLength(500);

            builder.Property(c => c.WebLink)
                .HasMaxLength(255);

            builder.HasMany(c => c.CompanyNumbers)
                .WithOne()
                .HasForeignKey(cn => cn.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Vacancies)
                .WithOne()
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}