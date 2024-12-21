using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Services.CityServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser)]
    public class CityController(ICityService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCity(CreateCityDto dto)
        {
            await service.CreateCityAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCity(string id, UpdateCityDto dto)
        {
            await service.UpdateCityAsync(id, dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCities()
        {
            var data = await service.GetAllCitiesAsync();
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCitiesByCountryId(string countryId)
        {
            return Ok(await service.GetAllCitiesByCountryIdAsync(countryId));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCity(string id)
        {
            await service.DeleteCityAsync(id);
            return Ok();
        }
    }
}