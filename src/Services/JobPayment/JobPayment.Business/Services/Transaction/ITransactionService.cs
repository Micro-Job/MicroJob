using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.TransactionSer
{
    public interface ITransactionService
    {
        public Task CreateTransactionAsync(CreateTransactionDto transaction);
    }
}
