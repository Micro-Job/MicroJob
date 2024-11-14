using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    internal class SavedVacancyConfiguration : IEntityTypeConfiguration<SavedVacancy>
    {
        public void Configure(EntityTypeBuilder<SavedVacancy> builder)
        {
            builder.HasOne(sv => sv.User)
              .WithMany(u => u.SavedVacancies)
              .HasForeignKey(sv => sv.UserId);

            builder.Property(sv => sv.VacancyId)
                   .IsRequired();
        }
    }
}