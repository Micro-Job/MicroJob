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
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class CountryController(ICountryService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCountry(CountryCreateDto dto)
        {
            await service.CreateCountryAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCountry(List<CountryUpdateDto> dtos)
        {
            await service.UpdateCountryAsync(dtos);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountries()
        {
            return Ok(await service.GetAllCountryAsync());
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPagedCountries(string? name, int skip = 1, int take = 5)
        {
            return Ok(await service.GetPagedCountriesAsync(name, skip, take));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountry(string id)
        {
            await service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}