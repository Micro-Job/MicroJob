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
        public async Task<IActionResult> Post([FromForm] ResumeCreateDto resumeCreateDto)
        {
            await _service.CreateAsync(resumeCreateDto);
            return Ok();
        }

        [HttpPost("ResumeUpdate")]
        public async Task<IActionResult> Put([FromForm] ResumeUpdateDto resumeUpdateDto)
        {
            await _service.UpdateAsync(resumeUpdateDto);
            return Ok();
        }

        [HttpGet("ResumeGetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("ResumeGetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }
    }
}