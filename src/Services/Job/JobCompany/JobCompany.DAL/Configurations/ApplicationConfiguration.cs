using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId).IsRequired();

            builder.HasOne(x => x.Vacancy)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Status)
               .WithMany(x => x.Applications)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}