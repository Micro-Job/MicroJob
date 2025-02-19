using Job.Business.Services.City;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController(ICityService cityService) : ControllerBase
    {
        private readonly ICityService _cityService = cityService;

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCities([FromQuery] Guid countryId, int skip = 1, int take = 6)
        {
            var data = await _cityService.GetAllCitiesAsync(countryId, skip, take);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountries([FromQuery] int skip = 1, int take = 6)
        {
            var data = await _cityService.GetAllCountriesAsync(skip, take);
            return Ok(data);
        }
    }
}