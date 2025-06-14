using Job.Business.Dtos.EducationDtos;

namespace Job.Business.Services.Education;

public interface IEducationService
{
    Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos,Guid resumeId);
    Task<ICollection<Core.Entities.Education>> UpdateBulkEducationAsync(ICollection<EducationUpdateDto> dtos, ICollection<Core.Entities.Education> existEducations, Guid resumeId);
}