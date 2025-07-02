using Job.Business.Dtos.SkillDtos;
using Job.Business.Services.Skill;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController(SkillService skillService) : ControllerBase
    {
        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSkill(SkillCreateDto dto)
        {
            await skillService.CreateSkillAsync(dto);
            return Ok();
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateSkil(SkillCreateDto dto)
        {
            await skillService.CreateSkillAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            return Ok(await skillService.GetAllSkillsAsync());
        }
    }
}