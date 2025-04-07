using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    public class PayEvent
    {
        public Guid UserId { get; set; }

        public Guid InformationId { get; set; }
        public InformationType InformationType { get; set; }
    }
}
