using JobPayment.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.DAL.Configurations
{
    public class PriceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasMany(x => x.OldPrices)
                .WithOne(x => x.Price)
                .HasForeignKey(x=> x.PriceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x=> x.InformationType).IsUnique();
        }
    }
}
