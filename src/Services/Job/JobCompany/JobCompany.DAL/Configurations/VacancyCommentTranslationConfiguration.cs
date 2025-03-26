using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class VacancyCommentTranslationConfiguration : IEntityTypeConfiguration<VacancyCommentTranslation>
{
    public void Configure(EntityTypeBuilder<VacancyCommentTranslation> builder)
    {
        builder.Property(x => x.Comment)
        .IsRequired()
        .HasMaxLength(128);

        builder.Property(x => x.Language)
            .IsRequired();

        builder.HasOne(x => x.VacancyComment)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.VacancyCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
