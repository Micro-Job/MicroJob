using JobPayment.Business.Dtos.DepositDtos;
using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.DepositServices
{
    public class DepositService(PaymentDbContext _context)
    {
        public async Task CreateDepositAsync(CreateDepositDto dto)
        {
            await _context.Deposits.AddAsync(new Deposit
            {
                Amount = dto.Amount,
                BalanceId = dto.BalanceId,
                TransactionId = dto.TransactionId,
            });

        }
    }
}
