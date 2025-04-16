using JobPayment.Business.Dtos.BalanceDtos;
using JobPayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.BalanceServices
{
    public interface IBalanceService
    {
        public Task IncreaseBalanceAsync(string packetId);
        public Task<Balance> GetOwnBalanceAsync();
        public Task<Balance> GetUserBalanceByIdAsync(Guid userId);
    }
}
