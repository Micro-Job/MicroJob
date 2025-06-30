using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Services.SkillServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class SkillController(SkillService _skillService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSkill(SkillCreateDto dto)
        {
            await _skillService.CreateSkillAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateSkill(List<SkillUpdateDto> dto)
        {
            await _skillService.UpdateSkillAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSkills()
        {
            var data = await _skillService.GetAllSkillsAsync();
            return Ok(data);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteSkill(string skillId)
        {
            await _skillService.DeleteSkillAsync(skillId);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetSkillsForSelect(string? skillName, int skip, int take = 5)
        {
            return Ok(await _skillService.GetSkillsForSelectAsync(skillName , skip , take));
        }
    }
}