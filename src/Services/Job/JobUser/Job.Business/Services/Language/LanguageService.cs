using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Language
{
    public class LanguageService(JobDbContext context) : ILanguageService
    {
        readonly JobDbContext _context = context;

        public async Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos, Guid resumeId)
        {
            var languagesToAdd = dtos.Select(dto => new Core.Entities.Language
            {
                ResumeId = resumeId,
                LanguageName = dto.LanguageName,
                LanguageLevel = dto.LanguageLevel
            }).ToList();

            await _context.Languages.AddRangeAsync(languagesToAdd);
            await _context.SaveChangesAsync();
            return languagesToAdd;
        }

        public async Task CreateLanguageAsync(LanguageCreateDto dto)
        {
            var language = new Core.Entities.Language
            {
                LanguageName = dto.LanguageName,
                LanguageLevel = dto.LanguageLevel
            };
            await _context.Languages.AddAsync(language);
        }

        public async Task<ICollection<Core.Entities.Language>> UpdateBulkLanguageAsync(ICollection<LanguageUpdateDto> dtos, Guid resumeId)
        {
            var languagesToUpdate = new List<Core.Entities.Language>();

            foreach (var dto in dtos)
            {
                var langId = Guid.Parse(dto.Id);
                var language = await _context.Languages
                    .FirstOrDefaultAsync(l => l.ResumeId == resumeId && l.Id == langId);

                if (language == null)
                {
                    throw new NotFoundException<Core.Entities.Language>();
                }

                language.LanguageName = dto.LanguageName;
                language.LanguageLevel = dto.LanguageLevel;

                languagesToUpdate.Add(language);
            }

            _context.Languages.UpdateRange(languagesToUpdate);
            await _context.SaveChangesAsync();

            return languagesToUpdate;
        }


        public async Task UpdateLanguageAsync(LanguageUpdateDto dto)
        {
            var language = await _context.Languages.FindAsync(dto.Id)
                ?? throw new NotFoundException<Core.Entities.Language>();
            language.LanguageName = dto.LanguageName;
            language.LanguageLevel = dto.LanguageLevel;
        }
    }
}