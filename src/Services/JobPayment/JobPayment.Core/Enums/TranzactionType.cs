using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Enums
{
    public enum TranzactionType : byte
    {
        InCome = 1,
        OutCome = 2,
        Rejected = 3
    }
}
