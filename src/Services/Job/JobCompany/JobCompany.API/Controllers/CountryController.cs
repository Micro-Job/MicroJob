using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Business.Services.CountryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(CountryService service) : ControllerBase
    {
        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCountry(CountryCreateDto dto)
        {
            await service.CreateCountryAsync(dto);
            return Ok();
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCountry(List<CountryUpdateDto> dtos)
        {
            await service.UpdateCountryAsync(dtos);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountries()
        {
            return Ok(await service.GetAllCountryAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPagedCountries(string? name, int skip = 1, int take = 5)
        {
            return Ok(await service.GetPagedCountriesAsync(name, skip, take));
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            await service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}