using JobPayment.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.TransactionDtos
{
    public class CreateTransactionDto
    {
        public Guid InformationId { get; set; }
        public InformationType InformationType { get; set; }
        public TranzactionType TranzactionType { get; set; }
        public decimal? Amount { get; set; }
        public double? Coin { get; set; }
        public double? BeforeBalanceCoin { get; set; }
        public Guid BalanceId { get; set; }
        public Guid UserId { get; set; }
    }
}
