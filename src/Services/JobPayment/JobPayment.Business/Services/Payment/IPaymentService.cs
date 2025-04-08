using JobPayment.Business.Dtos.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.Payment
{
    public interface IPaymentService
    {
        public Task Pay(PayDto dto);
    }
}
