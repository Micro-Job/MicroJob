using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Business.Services.PacketServices;
using JobPayment.Business.Services.TransactionServices;
using JobPayment.Core.Entities;
using JobPayment.Core.Enums;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;

namespace JobPayment.Business.Services.BalanceServices
{
    public class BalanceService(PaymentDbContext _context , ITransactionService _transactionService,  IPacketService _packetService , ICurrentUser _currentUser) : IBalanceService
    {
        public async Task IncreaseBalanceAsync(string packetId)
        {
            var existPacket = await _packetService.GetPacketByIdAsync(packetId);

            var myBalance = await GetOwnBalanceAsync();

            await _transactionService.CreateTransactionAsync(new CreateTransactionDto
            {
                BalanceId = myBalance.Id,
                Coin = existPacket.Coin,
                BeforeBalanceCoin = myBalance.Coin,
                TranzactionType = TranzactionType.InCome,
                InformationType = InformationType.PacketPayment,
                InformationId = existPacket.Id,
                UserId = (Guid)_currentUser.UserGuid,
                Amount = existPacket.Value
            });

            myBalance.Coin += existPacket.Coin;
                
            await _context.SaveChangesAsync();
        }

        public async Task<Balance> GetOwnBalanceAsync()
        {
            var myBalance = await _context.Balances.FirstOrDefaultAsync(x=> x.UserId == _currentUser.UserGuid) 
                            ?? throw new NotFoundException<Balance>();

            return myBalance;
        }
    }
}
