using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.BalanceSer
{
    public interface IBalanceService
    {
        public Task IncreaseBalanceAsync(string packetId);
    }
}
