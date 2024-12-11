using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Services.SkillServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSkills()
        {
            var data = await _skillService.GetAllSkillsAsync();
            return Ok(data);
        }
    }
}