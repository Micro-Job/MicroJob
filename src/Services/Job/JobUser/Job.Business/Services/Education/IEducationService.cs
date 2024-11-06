using Job.Business.Dtos.EducationDtos;

namespace Job.Business.Services.Education
{
    public interface IEducationService
    {
        Task CreateEducationAsync(EducationCreateDto dto);
        Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos);
        Task UpdateEducationAsync(string id, EducationUpdateDto dto);
    }
}