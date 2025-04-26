using JobPayment.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class Deposit : BaseEntity
    {
        public decimal? Amount { get; set; }

        public Guid BalanceId { get; set; }
        public Balance Balance { get; set; }
            
        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
