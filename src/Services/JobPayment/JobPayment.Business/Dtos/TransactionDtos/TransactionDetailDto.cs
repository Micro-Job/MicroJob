using JobPayment.Core.Enums;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.TransactionDtos
{
    public class TransactionDetailDto
    {
        public string? CompanyName { get; set; }
        public InformationType InformationType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public double? Coin { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
