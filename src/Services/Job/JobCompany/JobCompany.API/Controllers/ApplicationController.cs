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
        public async Task<IActionResult> CreateApplication(ApplicationCreateDto dto)
        {
            await _service.CreateApplicationAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplication(string vacancyId)
        {
            return Ok(await _service.GetAllApplicationAsync(vacancyId));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeApplicationStatus(string applicationId, string statusId)
        {
            await _service.ChangeApplicationStatusAsync(applicationId, statusId);
            return Ok();
        }
    }
}