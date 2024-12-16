using Job.Business.Dtos.LanguageDtos;

namespace Job.Business.Services.Language
{
    public interface ILanguageService
    {
        Task CreateLanguageAsync(LanguageCreateDto dto);
        Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos, Guid resumeId);
        Task UpdateLanguageAsync(LanguageUpdateDto dto);
        Task<ICollection<Core.Entities.Language>> UpdateBulkLanguageAsync(ICollection<LanguageUpdateDto> dtos, Guid resumeId);
    }
}