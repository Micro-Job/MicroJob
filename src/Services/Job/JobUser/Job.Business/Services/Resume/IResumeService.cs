using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Core.Enums;
using Shared.Enums;
using SharedLibrary.Enums;

namespace Job.Business.Services.Resume
{
    public interface IResumeService
    {
        Task CreateResumeAsync(ResumeCreateDto resumeCreateDto);
        Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto);
        Task<ResumeDetailItemDto> GetOwnResumeAsync();

        Task<DataListDto<ResumeListDto>> GetAllResumesAsync(
            string? fullname,
            bool? isPublic,
            ProfessionDegree? professionDegree,
            Citizenship? citizenship,
            bool? isExperience,
            JobStatus? jobStatus,
            List<string>? skillIds,
            List<LanguageFilterDto>? languages,
            int skip,
            int take);
        Task<DataListDto<ResumeListDto>> GetSavedResumesAsync(string? fullName , int skip , int take);
        Task<ResumeDetailItemDto> GetByIdResumeAysnc(string id);
        Task ToggleSaveResumeAsync(string resumeId);
        Task<bool> IsExistResumeAsync();

        Task TakeResumeAccessAsync(string resumeId);
    }
}
