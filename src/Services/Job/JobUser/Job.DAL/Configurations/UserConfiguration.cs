using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.UserFirstName)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(u => u.UserLastName)
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