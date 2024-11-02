using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Configurations
{
    public class PasswordTokenConfiguration : IEntityTypeConfiguration<PasswordToken>
    {
        public void Configure(EntityTypeBuilder<PasswordToken> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id);

            modelBuilder.HasOne(pt => pt.User)
                     .WithOne(u => u.PasswordToken) 
                     .HasForeignKey<PasswordToken>(pt => pt.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
