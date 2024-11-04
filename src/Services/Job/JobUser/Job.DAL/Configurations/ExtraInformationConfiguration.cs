using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class ExtraInformationConfiguration : IEntityTypeConfiguration<ExtraInformation>
    {
        public void Configure(EntityTypeBuilder<ExtraInformation> builder)
        {
            builder.HasOne(x => x.Resume)
                .WithMany(x => x.ExtraInformations)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}