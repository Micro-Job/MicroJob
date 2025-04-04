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
    public class DepositConfiguration : IEntityTypeConfiguration<Deposit>
    {
        public void Configure(EntityTypeBuilder<Deposit> builder)
        {
            builder.HasOne(x => x.Balance)
               .WithMany(x => x.Deposits)
               .HasForeignKey(x => x.BalanceId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Transaction)
               .WithOne(t => t.Deposit)
               .HasForeignKey<Deposit>(x => x.TransactionId)
               .OnDelete(DeleteBehavior.Restrict);
            ;
        }
    }
}
