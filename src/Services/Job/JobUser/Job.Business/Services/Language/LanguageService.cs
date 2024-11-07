using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;

namespace Job.Business.Services.Language
{
    public class LanguageService : ILanguageService
    {
        readonly JobDbContext _context;

        public LanguageService(JobDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Core.Entities.Language>> CreateBulkLanguageAsync(ICollection<LanguageCreateDto> dtos)
        {
            var languagesToAdd = dtos.Select(dto => new Core.Entities.Language
            {
                LanguageName = dto.LanguageName,
                LanguageLevel = dto.LanguageLevel
            }).ToList();

            await _context.Languages.AddRangeAsync(languagesToAdd);

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

        public async Task UpdateLanguageAsync(string id, LanguageUpdateDto dto)
        {
            var languageId = Guid.Parse(id);
            var language = await _context.Languages.FindAsync(languageId) ?? throw new NotFoundException<Core.Entities.Language>();
            language.LanguageName = dto.LanguageName;
            language.LanguageLevel = dto.LanguageLevel;
        }
    }
}