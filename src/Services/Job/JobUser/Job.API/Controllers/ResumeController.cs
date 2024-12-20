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
            // ICollection<CertificateCreateDto>? Certificates,
            ResumeCreateListsDto resumeCreateListsDto
        )
        {
            await _service.CreateResumeAsync(
                resumeCreateDto,
                // Certificates,
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