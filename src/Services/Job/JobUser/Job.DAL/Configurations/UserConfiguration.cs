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

            builder.HasMany(u => u.SavedVacancies)
                .WithOne(sv => sv.User)
                .HasForeignKey(sv => sv.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}