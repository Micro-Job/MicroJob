using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.Property(c => c.CertificateName)
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(c => c.GivenOrganization)
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(c => c.CertificateFile)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.HasOne(c => c.ExtraInformation)
                   .WithMany(e => e.Certificates)
                   .HasForeignKey(c => c.ExtraInformationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}