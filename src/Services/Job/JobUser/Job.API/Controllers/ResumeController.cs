using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        readonly IResumeService _service;

        public ResumeController(IResumeService service)
        {
            _service = service;
        }

        [HttpPost("ResumeCreate")]
        public async Task<IActionResult> CreateResume([FromForm] ResumeCreateDto resumeCreateDto)
        {
            await _service.CreateAsync(resumeCreateDto);
            return Ok();
        }
    }
}