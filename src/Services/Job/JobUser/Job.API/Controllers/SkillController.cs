using Job.Business.Dtos.SkillDtos;
using Job.Business.Services.Skill;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.SimpleUser)]
    public class SkillController(ISkillService skillService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSkill(SkillCreateDto dto)
        {
            await skillService.CreateSkillAsync(dto);
            return Ok();
        }

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