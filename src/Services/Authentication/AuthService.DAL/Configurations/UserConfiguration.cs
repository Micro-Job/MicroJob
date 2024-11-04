using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(u => u.MainPhoneNumber)
               .HasMaxLength(32)
               .IsRequired();

            modelBuilder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Property(e => e.Email)
                .HasMaxLength(100);

            modelBuilder.Property(e => e.RefreshToken)
                .HasMaxLength(500);

            modelBuilder.HasOne(e => e.UserStatus)
                .WithMany(us => us.Users)
                .HasForeignKey(e => e.UserStatusId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.HasMany(e => e.LoginLogs)
                .WithOne(log => log.User)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.HasOne(e => e.PasswordToken) 
                .WithOne(pt => pt.User)
                .HasForeignKey<PasswordToken>(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
