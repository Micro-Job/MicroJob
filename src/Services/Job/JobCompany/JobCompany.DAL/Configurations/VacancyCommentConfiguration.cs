using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class VacancyCommentConfiguration : IEntityTypeConfiguration<VacancyComment>
{
    public void Configure(EntityTypeBuilder<VacancyComment> builder)
    {

        builder.HasMany(x => x.Vacancies)
            .WithOne(v => v.VacancyComment)
            .HasForeignKey(v => v.VacancyCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
