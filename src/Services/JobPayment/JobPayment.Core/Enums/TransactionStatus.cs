using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Enums
{
    public enum TransactionStatus : byte
    {
        Success = 1,
        Failed = 2,
        Rejected = 3
    }
}
