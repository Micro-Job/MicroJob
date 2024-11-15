using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Services.CompanyServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCompanyAsync(CompanyUpdateDto dto)
        {
            await _service.UpdateCompanyAsync(dto);
            return Ok();
        }
    }
}