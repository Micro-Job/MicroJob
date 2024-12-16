using Job.Business.Dtos.EducationDtos;

namespace Job.Business.Services.Education
{
    public interface IEducationService
    {
        Task CreateEducationAsync(EducationCreateDto dto);
        Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos,Guid resumeId);
        Task UpdateEducationAsync(EducationUpdateDto dto);
        Task<ICollection<Core.Entities.Education>> UpdateBulkEducationAsync(ICollection<EducationUpdateDto> dtos, Guid resumeId);
    }
}