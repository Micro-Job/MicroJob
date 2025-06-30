using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

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

            builder.Property(x => x.VOEN)
            .HasMaxLength(16);

            builder.Property(u => u.MainPhoneNumber)
               .HasMaxLength(32)
               .IsRequired();

            builder.Property(u => u.Email)
               .HasMaxLength(100);

            builder.Property(c => c.CompanyLocation)
                .HasMaxLength(128);

            builder.Property(c => c.WebLink)
                .HasMaxLength(255);

            builder.HasMany(c => c.CompanyNumbers)
                .WithOne()
                .HasForeignKey(cn => cn.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Vacancies)
                .WithOne()
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Companies)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.City)
               .WithMany(x => x.Companies)
               .HasForeignKey(x => x.CityId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Companies)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.Statuses)
                .WithOne()
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.Receiver)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Exams)
                   .WithOne(e => e.Company)
                   .HasForeignKey(e => e.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CompanyName)
                .IsRequired(false)
                .HasMaxLength(32);
        }
    }
}