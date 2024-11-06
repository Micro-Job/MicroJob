using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Services.Education;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationsController : ControllerBase
    {
        readonly IEducationService _service;

        public EducationsController(IEducationService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateEducation([FromForm] EducationCreateDto dto)
        {
            await _service.CreateEducationAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateEducation(string id, [FromForm] EducationUpdateDto dto)
        {
            await _service.UpdateEducationAsync(id, dto);
            return Ok();
        }
    }
}