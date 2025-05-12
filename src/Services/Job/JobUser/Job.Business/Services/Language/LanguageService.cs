using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;

namespace Job.Business.Services.Language
{
    public class LanguageService(JobDbContext context) : ILanguageService
    {
        public async Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos, Guid resumeId)
        {
            var languagesToAdd = dtos.Select(dto => MapLanguageDtoToEntityForCreate(dto, resumeId)).ToList();

            await context.Languages.AddRangeAsync(languagesToAdd);
            await context.SaveChangesAsync();

            return languagesToAdd;
        }

        public async Task CreateLanguageAsync(LanguageCreateDto dto, Guid resumeId, bool saveChanges = true)
        {
            var language = MapLanguageDtoToEntityForCreate(dto, resumeId);

            await context.Languages.AddAsync(language);

            if (saveChanges) await context.SaveChangesAsync();
        }

        public async Task<ICollection<Core.Entities.Language>> UpdateBulkLanguageAsync(ICollection<LanguageUpdateDto> dtos, Guid resumeId)
        {
            var updatedLanguages = new List<Core.Entities.Language>();     // Nəticə olaraq qaytarılacaq dil siyahısı
            var newLanguageDtos = new List<LanguageCreateDto>();        // Yeni əlavə olunacaq dillər üçün yaradılacaq DTO siyahısı

            foreach (var dto in dtos)
            {
                if (Guid.TryParse(dto.Id, out var parsedId) && parsedId != Guid.Empty) // Əgər id düzgün parse olunursa
                {
                    var language = await context.Languages
                        .FirstOrDefaultAsync(x => x.Id == parsedId && x.ResumeId == resumeId)
                        ?? throw new NotFoundException<Core.Entities.Language>();

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
                    newLanguageDtos.Add(new LanguageCreateDto
                    {
                        LanguageName = dto.LanguageName,
                        LanguageLevel = dto.LanguageLevel
                    });
                }
            }

            if (newLanguageDtos.Count > 0) // Əgər yeni dillər varsa, onları əlavə edirik
            {
                var newlyCreated = await CreateBulkLanguageAsync(newLanguageDtos, resumeId);
                updatedLanguages.AddRange(newlyCreated);
            }

            await context.SaveChangesAsync();

            return updatedLanguages;
        }



        //TODO : saveChanges nedir?
        public async Task UpdateLanguageAsync(LanguageUpdateDto dto, Guid resumeId, bool saveChanges = true)
        {
            var parsedId = Guid.Parse(dto.Id);

            var language = await context.Languages
                .FirstOrDefaultAsync(x => x.Id == parsedId && x.ResumeId == resumeId)
                ?? throw new NotFoundException<Core.Entities.Language>();

            MapLanguageDtoToEntityForUpdate(language, dto);

            if (saveChanges) await context.SaveChangesAsync();
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
}