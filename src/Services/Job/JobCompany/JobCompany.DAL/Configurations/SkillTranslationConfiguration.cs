using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class SkillTranslationConfiguration : IEntityTypeConfiguration<SkillTranslation>
{
    public void Configure(EntityTypeBuilder<SkillTranslation> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Language)
            .IsRequired();

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
