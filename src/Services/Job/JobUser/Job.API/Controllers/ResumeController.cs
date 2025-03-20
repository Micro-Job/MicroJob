using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.SimpleUser)]
    public class ResumeController(IResumeService _resumeService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateResume(ResumeCreateDto resumeCreateDto)
        {
            await _resumeService.CreateResumeAsync(resumeCreateDto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateResume(ResumeUpdateDto resumeUpdateDto)
        {
            await _resumeService.UpdateResumeAsync(resumeUpdateDto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnResume()
        {
            return Ok(await _resumeService.GetOwnResumeAsync());
        }



        [HttpPost]
        public async Task<IActionResult> ToggleSaveResume(string resumeId)
        {
            await _resumeService.ToggleSaveResumeAsync(resumeId);
            return Ok();
        }
    }
}
