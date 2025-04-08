using JobPayment.Business.Dtos.PriceDtos;
using JobPayment.Core.Entities;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.PriceServices
{
    public interface IPriceService
    {
        public Task<List<PriceListDto>> GetAllPricesAsync();

        public Task UpdatePriceAsync(UpdatePriceDto dto);

        public Task<Service> GetPriceByInformationTypeAsync(InformationType type);
    }
}
