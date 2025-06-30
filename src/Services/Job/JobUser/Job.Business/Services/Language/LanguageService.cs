using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;

namespace Job.Business.Services.Language;

public class LanguageService(JobDbContext context)
{
    public async Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos, Guid resumeId)
    {
        var languagesToAdd = dtos.Select(dto => MapLanguageDtoToEntityForCreate(dto, resumeId)).ToList();

        await context.Languages.AddRangeAsync(languagesToAdd);
        await context.SaveChangesAsync();

        return languagesToAdd;
    }

    public async Task<ICollection<Core.Entities.Language>> UpdateBulkLanguageAsync(ICollection<LanguageUpdateDto> dtos, Guid resumeId)
    {
        var updatedLanguages = new List<Core.Entities.Language>();     // Nəticə olaraq qaytarılacaq dil siyahısı
        List<LanguageCreateDto>? newLanguageDtos = null;        // Yeni əlavə olunacaq dillər üçün yaradılacaq DTO siyahısı

        var existingLanguages = await context.Languages.Where(x => x.ResumeId == resumeId).ToListAsync();

        foreach (var dto in dtos)
        {
            if (Guid.TryParse(dto.Id, out var parsedId) && parsedId != Guid.Empty) // Əgər id düzgün parse olunursa
            {
                var language = existingLanguages.FirstOrDefault(x => x.Id == parsedId)
                    ?? throw new NotFoundException();

                MapLanguageDtoToEntityForUpdate(language, dto); // dil datasını güncəlləyirik

                updatedLanguages.Add(new Core.Entities.Language  // güncələnmiş dil geri qaytarılan siyahıya əlavə olunur
                {
                    Id = parsedId,
                    ResumeId = resumeId,
                    LanguageName = dto.LanguageName,
                    LanguageLevel = dto.LanguageLevel
                });
            }
            else // Əgər id düzgün parse olunmur və ya boşdursa, yeni dil əlavə edirik
            {
                newLanguageDtos ??= [];
                newLanguageDtos.Add(new LanguageCreateDto
                {
                    LanguageName = dto.LanguageName,
                    LanguageLevel = dto.LanguageLevel
                });
            }
        }

        if (newLanguageDtos?.Count > 0) // Əgər yeni dillər varsa, onları əlavə edirik
        {
            var newlyCreated = await CreateBulkLanguageAsync(newLanguageDtos, resumeId);
            updatedLanguages.AddRange(newlyCreated);
        }

        return updatedLanguages;
    }


    private static Core.Entities.Language MapLanguageDtoToEntityForCreate(LanguageCreateDto dto, Guid resumeId)
    {
        return new Core.Entities.Language
        {
            ResumeId = resumeId,
            LanguageName = dto.LanguageName,
            LanguageLevel = dto.LanguageLevel
        };
    }

    private static void MapLanguageDtoToEntityForUpdate(Core.Entities.Language language, LanguageUpdateDto dto)
    {
        language.LanguageName = dto.LanguageName;
        language.LanguageLevel = dto.LanguageLevel;
    }
}