using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Services.SkillServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class SkillController(ISkillService skillService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSkill(SkillCreateDto dto)
        {
            await skillService.CreateSkillAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateSkill(List<SkillUpdateDto> dto)
        {
            await skillService.UpdateSkillAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSkills()
        {
            var data = await skillService.GetAllSkillsAsync();
            return Ok(data);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteSkill(string skillId)
        {
            await skillService.DeleteSkillAsync(skillId);
            return Ok();
        }
    }
}