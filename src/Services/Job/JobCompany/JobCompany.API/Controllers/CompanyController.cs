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
    public class CompanyController(ICompanyService service) : ControllerBase
    {
        [HttpPut("[action]")]
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request)
        {
            await service.UpdateCompanyAsync(request.Dto, request.numbersDto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies(string? searchTerm = null,int skip = 1,int take = 6)
        {
            return Ok(await service.GetAllCompaniesAsync(searchTerm, skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetail(string companyId)
        {
            return Ok(await service.GetCompanyDetailAsync(companyId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnCompanyInformation()
        {
            return Ok(await service.GetOwnCompanyInformationAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyName(string companyId)
        {
            return Ok(await service.GetCompanyNameAsync(companyId));
        }
    }
}