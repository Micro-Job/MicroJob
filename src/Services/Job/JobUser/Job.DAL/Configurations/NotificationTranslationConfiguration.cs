using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations;

public class NotificationTranslationConfiguration : IEntityTypeConfiguration<NotificationTranslation>
{
    public void Configure(EntityTypeBuilder<NotificationTranslation> builder)
    {
        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Language)
            .IsRequired();

        builder.HasOne(x => x.Notification)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
