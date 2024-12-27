using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.HasKey(ua => ua.Id);

            builder.HasOne(ua => ua.User)
                   .WithMany(u => u.UserAnswers)
                   .HasForeignKey(ua => ua.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ua => ua.Text)
                   .HasMaxLength(256);

            builder.Property(ua => ua.IsCorrect)
                   .IsRequired(false);
        }
    }
}
