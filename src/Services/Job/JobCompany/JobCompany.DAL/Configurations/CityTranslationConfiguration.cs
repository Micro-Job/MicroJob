using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class CityTranslationConfiguration : IEntityTypeConfiguration<CityTranslation>
{
    public void Configure(EntityTypeBuilder<CityTranslation> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Language)
            .IsRequired();

        builder.HasOne(x => x.City)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
