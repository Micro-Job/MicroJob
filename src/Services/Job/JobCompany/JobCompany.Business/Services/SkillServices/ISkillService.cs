
using JobCompany.Business.Dtos.SkillDtos;

namespace JobCompany.Business.Services.SkillServices
{
    public interface ISkillService
    {
        Task CreateSkillAsync(SkillCreateDto dto);
        Task UpdateSkillAsync(List<SkillUpdateDto> dtos);
        Task<List<GetAllSkillDto>> GetAllSkillsAsync();
        Task DeleteSkillAsync(string skillId);
    }
}