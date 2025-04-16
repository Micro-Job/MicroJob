using JobPayment.Business.Dtos.Common;
using JobPayment.Business.Dtos.TransactionDtos;

namespace JobPayment.Business.Services.TransactionServices
{
    public interface ITransactionService
    {
        public Task CreateTransactionAsync(CreateTransactionDto transaction);

        public Task<DataListDto<TransactionListDto>> GetOwnTransactionsAsync(string? startDate , string? endDate , byte? transactionStatus , byte? informationType , byte? transactionType , int skip = 1 , int take = 7);

        public Task<TransactionDetailDto> GetTransactionDetailAsync(string transactionId);

        Task<List<TransactionDetailDto>> GetAllTransactionsByUserIdAsync(string userId);
    }
}
