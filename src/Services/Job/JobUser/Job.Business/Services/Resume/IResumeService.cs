using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        Task<ResumeDetailItemDto> GetOwnResumeAsync();
    }
}
