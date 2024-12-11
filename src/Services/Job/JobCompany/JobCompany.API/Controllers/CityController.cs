using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Services.CityServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController(ICityService service) : ControllerBase
    {
        readonly ICityService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCity(CreateCityDto dto)
        {
            await _service.CreateCityAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCity(string id, UpdateCityDto dto)
        {
            await _service.UpdateCityAsync(id, dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCities()
        {
            var data = await _service.GetAllCitiesAsync();
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCitiesByCountryId(string countryId)
        {
            return Ok(await _service.GetAllCitiesByCountryIdAsync(countryId));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCity(string id)
        {
            await _service.DeleteCityAsync(id);
            return Ok();
        }
    }
}