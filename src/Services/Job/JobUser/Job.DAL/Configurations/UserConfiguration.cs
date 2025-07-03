using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Job.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
            .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.MainPhoneNumber)
               .HasMaxLength(32)
               .IsRequired();

            builder.Property(u => u.Email)
              .HasMaxLength(100);

            builder.HasMany(u => u.Notifications)
                .WithOne(sv => sv.Receiver)
                .HasForeignKey(sv => sv.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            //builder.HasMany(x=> x.SavedResumes)
            //    .WithOne(x=> x.CompanyUser)
            //    .HasForeignKey(x=> x.CompanyUserId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}