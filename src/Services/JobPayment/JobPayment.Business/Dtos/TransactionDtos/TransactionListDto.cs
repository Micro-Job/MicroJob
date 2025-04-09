using JobPayment.Core.Enums;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.TransactionDtos
{
    public class TransactionListDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public double? Coin { get; set; }

        public Guid InformationId { get; set; }
        public InformationType InformationType { get; set; }
    }
}
