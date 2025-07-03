using JobPayment.Business.Dtos.PriceDtos;
using JobPayment.Business.Services.PriceServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController(PriceService _priceService) : ControllerBase
    {
        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPrices()
        {
            return Ok(await _priceService.GetAllPricesAsync());
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePrice(UpdatePriceDto dto)
        {
            await _priceService.UpdatePriceAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPrice(InformationType serviceType)
        {
            return Ok(await _priceService.GetPriceByInformationTypeAsync(serviceType));
        }
    }
}
