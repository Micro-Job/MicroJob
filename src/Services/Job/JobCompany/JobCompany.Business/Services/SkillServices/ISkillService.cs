
using JobCompany.Business.Dtos.SkillDtos;

namespace JobCompany.Business.Services.SkillServices
{
    public interface ISkillService
    {
        Task CreateSkillAsync(SkillDto dto);
        Task<List<GetAllSkillDto>> GetAllSkillsAsync();
    }
}