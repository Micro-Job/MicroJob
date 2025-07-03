using JobPayment.Core.Entities.Base;
using JobPayment.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class Packet : BaseEntity
    {
        public required string Title { get; set; }
        public PacketType PacketType { get; set; }
        public decimal Value { get; set; }
        public double Coin { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }


        public ICollection<OldPacket>? OldPackets { get; set; }
    }
}
