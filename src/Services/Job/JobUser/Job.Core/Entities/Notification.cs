 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Notification : BaseEntity
    {
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }

        public Guid SenderId { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? Content { get; set; }

        public bool IsSeen { get; set; }
    }
}
