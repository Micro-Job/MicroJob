using JobPayment.Business.Services.PacketSer;
using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.Balance
{
    public class BalanceService(PaymentDbContext _context , IPacketService _packetService , ICurrentUser _currentUser) : IBalanceService
    {
        public async Task IncreaseBalanceAsync(string packetId)
        {
            var existPacket = await _packetService.GetPacketByIdAsync(packetId);

            var myBalance = await GetOwnBalanceAsync();

            myBalance.Coin += existPacket.Coin;

            await _context.SaveChangesAsync();
        }

        public async Task<Core.Entities.Balance> GetOwnBalanceAsync()
        {
            var myBalance = await _context.Balances.FirstOrDefaultAsync(x=> x.Id == _currentUser.UserGuid) 
                            ?? throw new NotFoundException<Core.Entities.Balance>();

            return myBalance;
        }
    }
}
