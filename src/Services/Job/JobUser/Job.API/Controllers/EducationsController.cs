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

        [HttpPost("EducationCreate")]
        public async Task<IActionResult> Post([FromForm] EducationCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("EducationUpdate")]
        public async Task<IActionResult> Put(string id, [FromForm] EducationUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok();
        }
    }
}