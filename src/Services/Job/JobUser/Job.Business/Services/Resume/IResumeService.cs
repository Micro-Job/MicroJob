using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto,
        ResumeCreateListsDto resumeCreateListsDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        //Task<IEnumerable<ResumeListDto>> GetAllResumeAsync();
        Task<ResumeDetailItemDto> GetByIdResumeAsync();
    }
}