using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController(IResumeService service) : ControllerBase
    {
        readonly IResumeService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateResume([FromForm] ResumeCreateDto resumeCreateDto)
        {
            await _service.CreateResumeAsync(resumeCreateDto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateResume([FromForm] ResumeUpdateDto resumeUpdateDto)
        {
            await _service.UpdateResumeAsync(resumeUpdateDto);
            return Ok();
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllResume()
        //{
        //    return Ok(await _service.GetAllResumeAsync());
        //}

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByIdResume(string id)
        {
            return Ok(await _service.GetByIdResumeAsync(id));
        }
    }
}