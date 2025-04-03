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
    public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> builder)
        {
            builder.HasKey(x => x.UserId);

            builder.HasMany(x => x.Tranzactions)
                .WithOne(x => x.Balance)
                .HasForeignKey(x => x.Balance);
        }
    }
}
