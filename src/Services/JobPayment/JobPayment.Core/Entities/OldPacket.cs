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
        public Packet Packet { get; set; }
        public Guid PacketId { get; set; }

        public int MyProperty { get; set; }
    }
}
