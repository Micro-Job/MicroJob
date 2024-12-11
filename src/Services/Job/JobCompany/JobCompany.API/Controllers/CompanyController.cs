using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Services.CompanyServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController(ICompanyService service) : ControllerBase
    {
        readonly ICompanyService _service = service;

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCompany(CompanyUpdateDto dto)
        {
            await _service.UpdateCompanyAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies([FromQuery] string? searchTerm = null)
        {
            return Ok(await _service.GetAllCompaniesAsync(searchTerm));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetail(string companyId)
        {
            return Ok(await _service.GetCompanyDetailAsync(companyId));
        }
    }
}