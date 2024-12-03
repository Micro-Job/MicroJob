using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class UpdateUserApplicationStatusEvent
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}