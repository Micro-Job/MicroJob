using JobPayment.Business.Dtos.Common;
using JobPayment.Business.Dtos.DepositDtos;
using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Business.Services.DepositServices;
using JobPayment.Core.Entities;
using JobPayment.Core.Enums;
using JobPayment.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using SharedLibrary.Extensions;
using SharedLibrary.Helpers;
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
                CreatedDate = DateTime.Now.AddHours(4),
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
                    query = query.Where(x => x.CreatedDate >= Min);
            }

            if (endDate != null)
            {
                DateTime? Max = endDate.ToNullableDateTime();
                if (Max != null)
                    query = query.Where(x => x.CreatedDate <= Max);
            }

            if (transactionStatus != null)
                query = query.Where(x => x.TransactionStatus == (TransactionStatus)transactionStatus);

            if (transactionType != null)
                query = query.Where(x => x.TranzactionType == (TransactionType)transactionType);

            if (informationType != null)
                query = query.Where(x => x.InformationType == (InformationType)informationType);

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
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

            return new DataListDto<TransactionListDto>
            {
                Datas = transactions,
                TotalCount = await query.CountAsync(),
            };
        }

        public async Task<TransactionDetailDto> GetTransactionDetailAsync(string transactionId)
        {
            //TODO : bu hisse hem de admin ve ya operator terefinden islenir deye UserId silindi amma silinmeli deyil (muveqqeti hell)
            //var transaction = await _context.Transactions.Where(x => x.Id == Guid.Parse(transactionId) && x.UserId == _currentUser.UserGuid)

            var transaction = await _context.Transactions.Where(x => x.Id == Guid.Parse(transactionId))
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
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

            return transaction;
        }

        public async Task<DataListDto<TransactionListDto>> GetAllTransactionsByUserIdAsync(string userId, string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip, int take)
        {
            var query = _context.Transactions
                .Where(x => x.UserId == Guid.Parse(userId))
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsQueryable();

            if (startDate != null)
            {
                DateTime? Min = startDate.ToNullableDateTime();
                if (Min != null)
                    query = query.Where(x => x.CreatedDate >= Min);
            }

            if (endDate != null)
            {
                DateTime? Max = endDate.ToNullableDateTime();
                if (Max != null)
                    query = query.Where(x => x.CreatedDate <= Max);
            }

            if (transactionStatus != null)
                query = query.Where(x => x.TransactionStatus == (TransactionStatus)transactionStatus);

            if (transactionType != null)
                query = query.Where(x => x.TranzactionType == (TransactionType)transactionType);

            if (informationType != null)
                query = query.Where(x => x.InformationType == (InformationType)informationType);

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
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

            return new DataListDto<TransactionListDto>
            {
                Datas = transactions,
                TotalCount = await query.CountAsync(),
            };
        }

        public async Task<DataListDto<TransactionSummaryDto>> GetAllTransactionsAsync(string? searchTerm, string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip = 1, int take = 7)
        {
            var query = _context.Transactions
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (startDate != null)
            {
                DateTime? Min = startDate.ToNullableDateTime();
                if (Min != null)
                    query = query.Where(x => x.CreatedDate >= Min);
            }

            if (endDate != null)
            {
                DateTime? Max = endDate.ToNullableDateTime();
                if (Max != null)
                    query = query.Where(x => x.CreatedDate <= Max);
            }

            if (transactionStatus != null)
                query = query.Where(x => x.TransactionStatus == (TransactionStatus)transactionStatus);

            if (transactionType != null)
                query = query.Where(x => x.TranzactionType == (TransactionType)transactionType);

            if (informationType != null)
                query = query.Where(x => x.InformationType == (InformationType)informationType);

            int totalCount = await query.CountAsync();

            var transactions = await query.Select(x => new TransactionSummaryDto
            {
                Id = x.Id,
                Coin = x.Coin,
                CreatedDate = x.CreatedDate,
                InformationId = x.InformationId,
                InformationType = x.InformationType,
                TransactionStatus = x.TransactionStatus,
                TransactionType = x.TranzactionType,
                UserId = x.UserId,
                FullName = x.User.FirstName + " " + x.User.LastName
            })
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

            return new DataListDto<TransactionSummaryDto>
            {
                Datas = transactions,
                TotalCount = totalCount,
            };
        }
    }
}
