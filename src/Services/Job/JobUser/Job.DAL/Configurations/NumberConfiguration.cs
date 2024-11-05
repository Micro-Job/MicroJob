using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class NumberConfiguration : IEntityTypeConfiguration<Number>
    {
        public void Configure(EntityTypeBuilder<Number> builder)
        {
            builder.HasOne(n => n.Resume)
                   .WithMany(p => p.PhoneNumbers)
                   .HasForeignKey(n => n.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(32)
                .IsRequired();
        }
    }
}