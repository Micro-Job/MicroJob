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
            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne(x => x.User).WithOne(x => x.Balance).HasForeignKey<Balance>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Balance)
                .HasForeignKey(x => x.BalanceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
