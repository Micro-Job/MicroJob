using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Notifications)
                .WithOne(sv => sv.Receiver)
                .HasForeignKey(sv => sv.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x=> x.SavedResumes)
                .WithOne(x=> x.CompanyUser)
                .HasForeignKey(x=> x.CompanyUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}