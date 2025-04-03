using JobPayment.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class Balance : BaseEntity
    {
        public Guid UserId { get; set; }
        public double Coin { get; set; }
        public double BonusCoin { get; set; }
    }
}
