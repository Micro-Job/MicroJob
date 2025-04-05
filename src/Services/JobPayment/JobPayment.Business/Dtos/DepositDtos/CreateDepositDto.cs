using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.DepositDtos
{
    public class CreateDepositDto
    {
        public decimal? Amount { get; set; }
        public Guid UserId { get; set; }
        public Guid BalanceId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
