using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
using Job.Business.Dtos.ResumeDtos;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        Task<ResumeDetailItemDto> GetOwnResumeAsync();

        Task<DataListDto<ResumeListDto>> GetAllResumesAsync(string? fullname, int skip , int take);
        Task<DataListDto<SavedResumeListDto>> GetSavedResumesAsync(string? fullName , int skip , int take);
        Task ToggleSaveResumeAsync(string resumeId);

    }
}
