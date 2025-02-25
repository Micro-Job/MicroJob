using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

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
            foreach (var dto in dtos)
            {
                await UpdateLanguageAsync(dto, resumeId, false);
            }

            await context.SaveChangesAsync();

            var updatedLanguages = dtos.Select(dto => new Core.Entities.Language
            {
                Id = Guid.Parse(dto.Id),
                ResumeId = resumeId,
                LanguageName = dto.LanguageName,
                LanguageLevel = dto.LanguageLevel
            }).ToList();

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