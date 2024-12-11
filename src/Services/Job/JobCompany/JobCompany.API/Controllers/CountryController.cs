using JobCompany.Business.Services.CountryServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(ICountryService service) : ControllerBase
    {
        readonly ICountryService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCountry([FromBody] string countryName)
        {
            await _service.CreateCountryAsync(countryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCountry([FromRoute] string id, [FromBody] string? countryName)
        {
            await _service.UpdateCountryAsync(id, countryName);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountries()
        {
            return Ok(await _service.GetAllCountryAsync());
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountry(string id)
        {
            await _service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}