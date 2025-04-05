using JobPayment.Business.Dtos.DepositDtos;
using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Business.Services.DepositSer;
using JobPayment.Core.Entities;
using JobPayment.Core.Enums;
using JobPayment.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.TransactionSer
{
    public class TransactionService(PaymentDbContext _context , IDepositService _depositService) : ITransactionService
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
                InformationId = dto.InformationId,
                InformationType = dto.InformationType,
                TranzactionType = dto.TranzactionType,
            };

            await _context.Tranzactions.AddAsync(newTransaction);

            if(dto.TranzactionType == TranzactionType.InCome)
            {
                await _depositService.CreateDepositAsync(new CreateDepositDto
                {
                    Amount = dto.Amount,
                    BalanceId = dto.BalanceId,
                    UserId = dto.UserId,
                    TransactionId = newTransaction.Id
                }); 
            }
        }
    }
}
