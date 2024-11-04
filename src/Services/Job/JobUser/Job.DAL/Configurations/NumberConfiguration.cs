using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class NumberConfiguration : IEntityTypeConfiguration<Number>
    {
        public void Configure(EntityTypeBuilder<Number> builder)
        {
            builder.HasOne(n => n.Person)
                   .WithMany(p => p.PhoneNumbers)
                   .HasForeignKey(n => n.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}