using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.DAL.Configurations
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.Property(x => x.StatusName)
                .IsRequired()
                .HasMaxLength(32);

            builder.HasOne(x=>x.Company)
                .WithMany(x=>x.Statuses)
                .HasForeignKey(x=>x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
