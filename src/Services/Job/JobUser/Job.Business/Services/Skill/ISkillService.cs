using Job.Business.Dtos.SkillDtos;

namespace Job.Business.Services.Skill
{
    public interface ISkillService
    {
        Task CreateSkillAsync(SkillDto dto);
        Task<List<GetAllSkillDto>> GetAllSkillsAsync();
    }
}