using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasOne(x => x.ExtraInformation)
                .WithMany(x => x.Languages)
                .HasForeignKey(x => x.ExtraInformationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}