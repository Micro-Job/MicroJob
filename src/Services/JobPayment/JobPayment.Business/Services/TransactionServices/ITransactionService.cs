using JobPayment.Business.Dtos.Common;
using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.TransactionServices
{
    public interface ITransactionService
    {
        public Task CreateTransactionAsync(CreateTransactionDto transaction);

        public Task<DataListDto<TransactionListDto>> GetOwnTransactionsAsync(string? startDate , string? endDate , byte? transactionStatus , byte? informationType , byte? transactionType , int skip = 1 , int take = 7);
    }
}
