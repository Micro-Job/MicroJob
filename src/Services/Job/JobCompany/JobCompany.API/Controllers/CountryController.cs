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
        public async Task<IActionResult> CreateCountry([FromBody] string countryName)
        {
            await service.CreateCountryAsync(countryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCountry([FromRoute] string id, [FromBody] string? countryName)
        {
            await service.UpdateCountryAsync(id, countryName);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountries()
        {
            return Ok(await service.GetAllCountryAsync());
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountry(string id)
        {
            await service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}