
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.SkillDtos;

namespace JobCompany.Business.Services.SkillServices
{
    public interface ISkillService
    {
        Task CreateSkillAsync(SkillCreateDto dto);
        Task UpdateSkillAsync(List<SkillUpdateDto> dtos);
        Task<List<GetAllSkillDto>> GetAllSkillsAsync();
        Task<DataListDto<GetAllSkillDto>> GetSkillsForSelectAsync(string? skillName, int skip, int take = 5);
        Task DeleteSkillAsync(string skillId);
    }
}