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
        public double Coin { get; set; }
        /// <summary>
        /// Hele ki deqiqlesmeyib
        /// </summary>
        public double? BonusCoin { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public ICollection<Transaction>? Tranzactions { get; set; }
        public ICollection<Deposit>? Deposits { get; set; }
    }
}
