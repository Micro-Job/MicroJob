using Job.Business.Dtos.SkillDtos;
using Job.Business.Services.Skill;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController(ISkillService skillService) : ControllerBase
    {
        private readonly ISkillService _skillService = skillService;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSkill(SkillDto dto)
        {
            await _skillService.CreateSkillAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            return Ok(await _skillService.GetAllSkillsAsync());
        }
    }
}