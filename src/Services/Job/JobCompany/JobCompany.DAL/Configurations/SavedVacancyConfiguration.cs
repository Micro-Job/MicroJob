using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.DAL.Configurations
{
    public class SavedVacancyConfiguration : IEntityTypeConfiguration<SavedVacancy>
    {
        public void Configure(EntityTypeBuilder<SavedVacancy> builder)
        {
            builder.HasOne(sv => sv.Vacancy)
              .WithMany(u => u.SavedVacancies)
              .HasForeignKey(sv => sv.VacancyId);

            builder.Property(sv => sv.VacancyId)
                   .IsRequired();

            builder.Property(sv => sv.UserId)
                   .IsRequired();
        }
    }
}
