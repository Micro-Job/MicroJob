using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    public class CreateBalanceEvent
    {
        public Guid UserId { get; set; }
    }
}
