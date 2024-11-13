using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class VacancyTestConfiguration : IEntityTypeConfiguration<VacancyTest>
    {
        public void Configure(EntityTypeBuilder<VacancyTest> builder)
        {
            //other properties
            builder.HasKey(p => p.Id);

            builder.HasMany(vt => vt.Vacancies)
                   .WithOne(v => v.VacancyTest)
                   .HasForeignKey(v => v.VacancyTestId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}