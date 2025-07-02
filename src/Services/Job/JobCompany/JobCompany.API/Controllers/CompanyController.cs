using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Services.CompanyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController(CompanyService service) : ControllerBase
    {
        [HttpPut("[action]")]
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        public async Task<IActionResult> UpdateCompany([FromForm] UpdateCompanyRequest request)
        {
            return Ok(await service.UpdateCompanyAsync(request.CompanyDto, request.NumbersDto));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies(string? searchTerm, bool? countDesc, int skip = 1, int take = 6)
        {
            return Ok(await service.GetAllCompaniesAsync(searchTerm, countDesc, skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetail(Guid companyId)
        {
            return Ok(await service.GetCompanyDetailAsync(companyId));
        }

        [AuthorizeRole(UserRole.EmployeeUser, UserRole.CompanyUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnCompanyInformation()
        {
            return Ok(await service.GetOwnCompanyInformationAsync());
        }

    }
}