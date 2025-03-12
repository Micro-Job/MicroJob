using Job.Business.Dtos.SkillDtos;

namespace Job.Business.Services.Skill;

public interface ISkillService
{
    Task CreateSkillAsync(SkillCreateDto dto);
    Task UpdateSkillAsync(List<SkillUpdateDto> dto);
    Task<List<GetAllSkillDto>> GetAllSkillsAsync();
}