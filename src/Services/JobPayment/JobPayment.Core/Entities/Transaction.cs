
using JobPayment.Core.Entities.Base;
using JobPayment.Core.Enums;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid InformationId { get; set; }
        /// <summary>
        /// Neye gore tranzaksiyanin bas verdiyini bilmek ucun
        /// </summary>
        public InformationType InformationType { get; set; }

        /// <summary>
        /// Tranzaksiyanin novu(Medaxil,mexaric,legv ve s.)
        /// </summary>
        public TransactionType TranzactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        /// <summary>
        /// Tranzaksiya zamani ne qeder coin emeliyyati bas verib
        /// </summary>
        public double? Coin { get; set; }
        public double? BeforeBalanceCoin { get; set; }
        public DateTime? CreatedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid BalanceId { get; set; }
        public Balance Balance { get; set; }



        //Mərkəz olaraq deposit görülüb deyə Transaction-ı depositdə saxlayırıq.Bu istifadəyə görə dəyişə bilər(one-to-one)
        public Deposit Deposit { get; set; }
    }
}
