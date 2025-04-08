using JobPayment.Business.Dtos.PriceDtos;
using JobPayment.Business.Services.PriceServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController(IPriceService _priceService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPrices()
        {
            return Ok(await _priceService.GetAllPricesAsync());
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePrice(UpdatePriceDto dto)
        {
            await _priceService.UpdatePriceAsync(dto);
            return Ok();
        }
    }
}
