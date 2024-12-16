using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class VacancyNumberConfiguration : IEntityTypeConfiguration<VacancyNumber>
    {
        public void Configure(EntityTypeBuilder<VacancyNumber> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(cn => cn.Number)
                   .HasMaxLength(32);

            builder.HasOne(cn => cn.Vacancy)
                   .WithMany(v => v.VacancyNumbers)
                   .HasForeignKey(cn => cn.VacancyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}