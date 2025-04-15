using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Requests
{
    public class CheckBalanceRequest
    {
        public Guid UserId { get; set; }
        public InformationType InformationType { get; set; }
    }
}
