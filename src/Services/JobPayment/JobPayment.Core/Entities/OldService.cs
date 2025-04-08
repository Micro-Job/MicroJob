using JobPayment.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class OldService : BaseEntity
    {
        public double OldCoin { get; set; }

        public Guid PriceId { get; set; }
        public Service Price { get; set; }
    }
}
