using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Job.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllResumes([FromQuery] string? fullname,
            [FromQuery] bool? isPublic,
            [FromQuery] ProfessionDegree? professionDegree,
            [FromQuery] Citizenship? citizenship,
            [FromQuery] bool? isExperience,
            [FromQuery] JobStatus? jobStatus,
            [FromQuery] List<string>? skillIds,
            [FromQuery] List<LanguageFilterDto>? languages,
            [FromQuery] int skip = 1, [FromQuery] int take = 9)
        {
            return Ok(await _resumeService.GetAllResumesAsync(fullname,
                isPublic, professionDegree, citizenship,isExperience,jobStatus, skillIds,languages,
                skip,take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetByIdResume(string id)
        {
            return Ok(await _resumeService.GetByIdResumeAysnc(id));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSavedResumes(string? fullname, int skip = 1, int take = 9)
        {
            return Ok(await _resumeService.GetSavedResumesAsync(fullname, skip, take));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleSaveResume(string resumeId)
        {
            await _resumeService.ToggleSaveResumeAsync(resumeId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> IsExistResume()
        {
            return Ok(await _resumeService.IsExistResumeAsync());
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> TakeResumeAccess(string resumeId)
        {
            await _resumeService.TakeResumeAccessAsync(resumeId);
            return Ok();
        }

    }
}
