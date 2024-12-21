using Job.Business.Dtos.CertificateDtos;
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
        public async Task<IActionResult> CreateResume(
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

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateResume(ResumeUpdateDto resumeUpdateDto,
            ResumeUpdateListDto resumeUpdateListsDto)
        {
            await _service.UpdateResumeAsync(resumeUpdateDto, resumeUpdateListsDto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnResume()
        {
            return Ok(await _service.GetOwnResumeAsync());
        }
    }
}