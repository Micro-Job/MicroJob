using Job.Business.Services.Company;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController(ICompanyInformationService service) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompaniesDataAsync(string? searchTerm)
        {
            return Ok(await service.GetCompaniesDataAsync(searchTerm));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetCompanyDetailByIdAsync(string id)
        {
            return Ok(await service.GetCompanyDetailByIdAsync(id));
        }
    }
}