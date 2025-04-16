using JobPayment.Business.Dtos.Common;
using JobPayment.Business.Dtos.DepositDtos;
using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Business.Services.DepositServices;
using JobPayment.Core.Entities;
using JobPayment.Core.Enums;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using SharedLibrary.Extensions;
using SharedLibrary.HelperServices.Current;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.TransactionServices
{
    public class TransactionService(PaymentDbContext _context, IDepositService _depositService, ICurrentUser _currentUser) : ITransactionService
    {
        public async Task CreateTransactionAsync(CreateTransactionDto dto)
        {
            var newTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Coin = dto.Coin,
                UserId = dto.UserId,
                BalanceId = dto.BalanceId,
                CreatedDate = DateTime.Now,
                BeforeBalanceCoin = dto.BeforeBalanceCoin,
                TransactionStatus = dto.TransactionStatus,
                InformationId = dto.InformationId,
                InformationType = dto.InformationType,
                TranzactionType = dto.TranzactionType,
            };

            await _context.Transactions.AddAsync(newTransaction);

            if (dto.TranzactionType == TransactionType.InCome && dto.Amount != null)
            {
                await _depositService.CreateDepositAsync(new CreateDepositDto
                {
                    Amount = dto.Amount,
                    BalanceId = dto.BalanceId,
                    TransactionId = newTransaction.Id
                });
            }
        }

        public async Task<DataListDto<TransactionListDto>> GetOwnTransactionsAsync(string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip = 1, int take = 7)
        {
            var query = _context.Transactions
                .Where(x => x.UserId == _currentUser.UserGuid)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsQueryable();

            if (startDate != null)
            {
                DateTime? Min = startDate.ToNullableDateTime();
                if (Min != null)
                {
                    query = query.Where(x => x.CreatedDate >= Min);
                }
            }

            if (endDate != null)
            {
                DateTime? Max = endDate.ToNullableDateTime();
                if (Max != null)
                {
                    query = query.Where(x => x.CreatedDate <= Max);
                }
            }

            if (transactionStatus != null)
                query = query.Where(x => x.TransactionStatus == (TransactionStatus)transactionStatus);

            if (transactionType != null)
                query = query.Where(x => x.TranzactionType == (TransactionType)transactionType);

            if (informationType != null)
                query = query.Where(x => x.InformationType == (InformationType)informationType);

            int count = await query.CountAsync();

            var transactions = await query.Select(x => new TransactionListDto
            {
                Id = x.Id,
                Coin = x.Coin,
                CreatedDate = x.CreatedDate,
                InformationId = x.InformationId,
                InformationType = x.InformationType,
                TransactionStatus = x.TransactionStatus,
                TransactionType = x.TranzactionType

            })
            .Skip(Math.Max(0, skip - 1) * take)
            .Take(take)
            .ToListAsync();

            return new DataListDto<TransactionListDto>
            {
                Datas = transactions,
                TotalCount = count,
                TotalPage = (int)Math.Ceiling((double)count / take)
            };
        }

        public async Task<TransactionDetailDto> GetTransactionDetailAsync(string transactionId)
        {
            var transaction = await _context.Transactions.Where(x => x.Id == Guid.Parse(transactionId) && x.UserId == _currentUser.UserGuid)
            .Select(x => new TransactionDetailDto
            {
                CompanyName = null,
                Amount = x.Deposit.Amount,
                Coin = x.Coin,
                InformationId = x.InformationId,
                InformationType = x.InformationType,
                TransactionType = x.TranzactionType,
                TransactionStatus = x.TransactionStatus,
                TransactionDate = x.CreatedDate
            }).FirstOrDefaultAsync() ?? throw new NotFoundException<Transaction>("Əməliyyat mövcud deyil");

            return transaction;
        }

        public async Task<List<TransactionDetailDto>> GetAllTransactionsByUserIdAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var transactions = await _context.Transactions.Where(x => x.UserId == userGuid)
            .Select(x => new TransactionDetailDto
            {
                CompanyName = null,
                Amount = x.Deposit.Amount,
                Coin = x.Coin,
                InformationId = x.InformationId,
                InformationType = x.InformationType,
                TransactionType = x.TranzactionType,
                TransactionStatus = x.TransactionStatus,
                TransactionDate = x.CreatedDate
            }).ToListAsync();

            return transactions;    
        }
    }
}
