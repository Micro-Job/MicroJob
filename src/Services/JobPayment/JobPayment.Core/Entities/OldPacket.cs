using JobPayment.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class OldPacket : BaseEntity
    {
        public decimal OldValue { get; set; }
        public double Coin { get; set; }
        /// <summary>
        /// Packetden OldPackete dusme tarixi
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public Guid PacketId { get; set; }
        public Packet Packet { get; set; }
    }
}
