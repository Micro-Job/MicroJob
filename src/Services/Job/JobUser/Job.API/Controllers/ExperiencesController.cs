using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Services.Experience;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperiencesController : ControllerBase
    {
        readonly IExperienceService _service;

        public ExperiencesController(IExperienceService service)
        {
            _service = service;
        }

        [HttpPost("ExperienceCreate")]
        public async Task<IActionResult> Post([FromForm] ExperienceCreateDto experienceCreateDto)
        {
            await _service.CreateAsync(experienceCreateDto);
            return Ok();
        }

        [HttpPut("ExperienceUpdate")]
        public async Task<IActionResult> Put(string id, [FromForm] ExperienceUpdateDto experienceUpdateDto)
        {
            await _service.UpdateAsync(id, experienceUpdateDto);
            return Ok();
        }
    }
}