using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.CreatedDate);

            builder.Property(x => x.Content)
                .HasMaxLength(500)  
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
