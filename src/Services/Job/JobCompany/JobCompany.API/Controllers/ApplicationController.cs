using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        readonly IApplicationService _service;

        public ApplicationController(IApplicationService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateApplicationAsync(ApplicationCreateDto dto)
        {
            await _service.CreateApplicationAsync(dto);
            return Ok();
        }
    }
}