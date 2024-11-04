using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.Property(p => p.FatherName)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.Property(p => p.UserPhoto)
                   .HasMaxLength(255);

            builder.Property(p => p.Adress)
                   .HasMaxLength(100);

            builder.Property(p => p.BirthDay)
                   .IsRequired();

            builder.HasOne(p => p.User)
                   .WithOne()
                   .HasForeignKey<Person>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PhoneNumbers)
                   .WithOne(n => n.Person)
                   .HasForeignKey(n => n.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}