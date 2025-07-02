using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Services.CityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController(CityService service) : ControllerBase
    {
        [AuthorizeRole(UserRole.SuperAdmin)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCity(CreateCityDto dto)
        {
            await service.CreateCityAsync(dto);
            return Ok();
        }

        [AuthorizeRole(UserRole.SuperAdmin)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCity(List<UpdateCityDto> dtos)
        {
            await service.UpdateCityAsync(dtos);
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCitiesByCountryIds([FromQuery] List<string> countryIds, string? name, int skip = 1, int take = 5)
        {
            return Ok(await service.GetAllCitiesByCountryIdsAsync(countryIds, name, skip, take));
        }


        //[AuthorizeRole(UserRole.SuperAdmin)]
        //[HttpDelete("[action]/{id}")]
        //public async Task<IActionResult> DeleteCity(string id)
        //{
        //    await service.DeleteCityAsync(id);
        //    return Ok();
        //}
    }
}