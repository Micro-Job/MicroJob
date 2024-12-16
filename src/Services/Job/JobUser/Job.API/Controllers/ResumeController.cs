using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResumeController(IResumeService service) : ControllerBase
    {
        readonly IResumeService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateResumeAsync(
            ResumeCreateDto resumeCreateDto,
            ResumeCreateListsDto resumeCreateListsDto
        )
        {
            await _service.CreateResumeAsync(
                resumeCreateDto,
                resumeCreateListsDto
            );
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateResume([FromForm] ResumeUpdateDto resumeUpdateDto)
        {
            await _service.UpdateResumeAsync(resumeUpdateDto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetByIdResume()
        {
            return Ok(await _service.GetByIdResumeAsync());
        }
    }
}