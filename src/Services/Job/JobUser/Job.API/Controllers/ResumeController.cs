using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.SimpleUser)]
    public class ResumeController(IResumeService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateResume([FromForm] ResumeCreateDto resumeCreateDto, 
            [FromForm] ResumeCreateListsDto resumeCreateListsDto)
        {
            await service.CreateResumeAsync(resumeCreateDto, resumeCreateListsDto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateResume([FromForm] ResumeUpdateDto resumeUpdateDto,
            [FromForm] ResumeUpdateListDto resumeUpdateListsDto)
        {
            await service.UpdateResumeAsync(resumeUpdateDto, resumeUpdateListsDto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnResume()
        {
            return Ok(await service.GetOwnResumeAsync());
        }
    }
}
