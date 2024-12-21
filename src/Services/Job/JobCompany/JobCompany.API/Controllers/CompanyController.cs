using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Services.CompanyServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController(ICompanyService service) : ControllerBase
    {
        [HttpPut("[action]")]
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request)
        {
            await service.UpdateCompanyAsync(request.Dto, request.NumbersDto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies([FromQuery] string? searchTerm = null)
        {
            return Ok(await service.GetAllCompaniesAsync(searchTerm));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetail(string companyId)
        {
            return Ok(await service.GetCompanyDetailAsync(companyId));
        }
    }
}