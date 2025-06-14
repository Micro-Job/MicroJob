using Job.Business.Dtos.ExperienceDtos;

namespace Job.Business.Services.Experience;

public interface IExperienceService
{
    Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(ICollection<ExperienceCreateDto> dtos,Guid resumeId);
    Task<ICollection<Core.Entities.Experience>> UpdateBulkExperienceAsync(ICollection<ExperienceUpdateDto> updateDtos, ICollection<Core.Entities.Experience> existingExperiences, Guid resumeId);
}