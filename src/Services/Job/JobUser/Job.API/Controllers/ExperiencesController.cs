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

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExperience([FromForm] ExperienceCreateDto experienceCreateDto)
        {
            await _service.CreateExperienceAsync(experienceCreateDto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateExperience(string id, [FromForm] ExperienceUpdateDto experienceUpdateDto)
        {
            await _service.UpdateExperienceAsync(id, experienceUpdateDto);
            return Ok();
        }
    }
}