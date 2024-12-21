using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Services.Company;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        readonly ICompanyInformationService _service;

        public CompanyController(ICompanyInformationService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompaniesDataAsync()
        {
            return Ok(await _service.GetCompaniesDataAsync());
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetCompanyDetailByIdAsync(string id)
        {
            return Ok(await _service.GetCompanyDetailByIdAsync(id));
        }
    }
}