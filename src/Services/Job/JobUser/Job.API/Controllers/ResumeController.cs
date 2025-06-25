using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Services.Resume;
using Job.Core.Entities;
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
    public class ResumeController(ResumeService _resumeService) : ControllerBase
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
            [FromQuery] List<ProfessionDegree>? professionDegree,
            [FromQuery] Citizenship? citizenship,
            [FromQuery] Gender? gender,
            [FromQuery] bool? isExperience,
            [FromQuery] JobStatus? jobStatus,
            [FromQuery] List<Guid>? skillIds,
            [FromQuery] List<LanguageFilterDto>? languages,
            [FromQuery] int skip = 1, [FromQuery] int take = 9)
        {
            return Ok(await _resumeService.GetAllResumesAsync(fullname,
                isPublic, professionDegree, citizenship, gender, isExperience, jobStatus, skillIds, languages,
                skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetByIdResume(Guid id)
        {
            return Ok(await _resumeService.GetByIdResumeAysnc(id));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSavedResumes([FromQuery] string? fullname,
            [FromQuery] bool? isPublic,
            [FromQuery] List<ProfessionDegree>? professionDegree,
            [FromQuery] Citizenship? citizenship,
            [FromQuery] Gender? gender,
            [FromQuery] bool? isExperience,
            [FromQuery] JobStatus? jobStatus,
            [FromQuery] List<Guid>? skillIds,
            [FromQuery] List<LanguageFilterDto>? languages,
            [FromQuery] int skip = 1, [FromQuery] int take = 9)
        {
            return Ok(await _resumeService.GetSavedResumesAsync(fullname, isPublic, jobStatus, professionDegree, citizenship, gender, isExperience, skillIds, languages, skip, take));
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
