using Job.Business.Dtos.LanguageDtos;

namespace Job.Business.Services.Language
{
    public interface ILanguageService
    {
        Task CreateLanguageAsync(LanguageCreateDto dto, Guid resumeId, bool saveChanges = true);
        Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos, Guid resumeId);
        Task UpdateLanguageAsync(LanguageUpdateDto dto, Guid resumeId, bool saveChanges = true);
        Task<ICollection<Core.Entities.Language>> UpdateBulkLanguageAsync(ICollection<LanguageUpdateDto> dtos, Guid resumeId);
    }
}