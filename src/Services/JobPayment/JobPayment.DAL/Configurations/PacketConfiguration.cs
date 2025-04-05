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
    public class PacketConfiguration : IEntityTypeConfiguration<Packet>
    {
        public void Configure(EntityTypeBuilder<Packet> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(255);

            builder.Property(x => x.Value).IsRequired();
            builder.Property(x => x.Coin).IsRequired();

            builder.HasMany(x => x.OldPackets)
                .WithOne(x => x.Packet)
                .HasForeignKey(x => x.PacketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
