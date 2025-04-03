using JobPayment.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.DAL.Contexts
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions options) : base(options) { }


        public DbSet<Balance> Balances { get; set; }
        public DbSet<OldPacket> OldPackets { get; set; }
        public DbSet<Packet> Packets { get; set; }
        public DbSet<Transaction> Tranzactions { get; set; }
    }
}
