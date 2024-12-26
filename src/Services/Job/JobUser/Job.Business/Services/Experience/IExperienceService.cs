using Job.Business.Dtos.ExperienceDtos;

namespace Job.Business.Services.Experience
{
    public interface IExperienceService
    {
        Task CreateExperienceAsync(ExperienceCreateDto dto, Guid resumeId, bool saveChanges = true);
        Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(ICollection<ExperienceCreateDto> dtos,Guid resumeId);
        Task UpdateExperienceAsync(ExperienceUpdateDto dto, Guid resumeId, bool saveChanges = true);
        Task<ICollection<Core.Entities.Experience>> UpdateBulkExperienceAsync(ICollection<ExperienceUpdateDto> dtos,Guid resumeId);
    }
}