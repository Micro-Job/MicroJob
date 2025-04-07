using JobPayment.Business.Dtos.DepositDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.DepositServices
{
    public interface IDepositService
    {
        public Task CreateDepositAsync(CreateDepositDto dto);
    }
}
