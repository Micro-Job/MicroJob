using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(u => u.LastName)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(u=>u.Email)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(u => u.MainPhoneNumber)
               .HasMaxLength(32)
               .IsRequired();
        }
    }
}