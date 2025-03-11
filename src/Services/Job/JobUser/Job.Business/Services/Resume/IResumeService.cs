using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(
            ResumeCreateDto resumeCreateDto,
            ResumeCreateListsDto resumeCreateListsDto
        );
        Task UpdateResumeAsync(
            ResumeUpdateDto resumeUpdateDto,
            ResumeUpdateListDto resumeUpdateListsDto
        );
        Task<ResumeDetailItemDto> GetOwnResumeAsync(string salam);
    }
}
