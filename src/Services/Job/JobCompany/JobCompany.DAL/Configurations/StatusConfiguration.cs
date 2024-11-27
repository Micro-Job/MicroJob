using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StatusName)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.Order);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Statuses)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Applications)
                .WithOne(v => v.Status)
                .HasForeignKey(v => v.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}