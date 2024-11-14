using Job.Business.Dtos.ExperienceDtos;

namespace Job.Business.Services.Experience
{
    public interface IExperienceService
    {
        Task CreateExperienceAsync(ExperienceCreateDto dto);
        Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(ICollection<ExperienceCreateDto> dtos,Guid resumeId);
        Task UpdateExperienceAsync(string id, ExperienceUpdateDto dto);
    }
}