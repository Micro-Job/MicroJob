using JobCompany.Business.Services.CountryServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        readonly ICountryService _service;

        public CountryController(ICountryService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCountryAsync([FromBody] string countryName)
        {
            await _service.CreateCountryAsync(countryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCountryAsync([FromRoute] string id, [FromBody] string? countryName)
        {
            await _service.UpdateCountryAsync(id, countryName);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountriesAsync()
        {
            return Ok(await _service.GetAllCountryAsync());
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountryAsync(string id)
        {
            await _service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}