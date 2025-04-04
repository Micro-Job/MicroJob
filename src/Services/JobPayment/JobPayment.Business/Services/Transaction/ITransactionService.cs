using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.Transaction
{
    public interface ITransactionService
    {
        public Task CreateTransaction();
    }
}
