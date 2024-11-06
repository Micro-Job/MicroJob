using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Exceptions.Common;
using Job.Core.Entities;
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

        public async Task CreateLanguageAsync(LanguageCreateDto dto)
        {
            var resumeId = Guid.Parse(dto.ResumeId);
            var resume = await _context.Resumes.FindAsync(resumeId);
            if (resume == null) throw new NotFoundException<Core.Entities.Resume>();

            var language = new Core.Entities.Language
            {
                ResumeId = resumeId,
                LanguageName = dto.LanguageName,
                LanguageLevel = dto.LanguageLevel
            };

            await _context.Languages.AddAsync(language);
        }

        public async Task UpdateLanguageAsync(string id, LanguageUpdateDto dto)
        {
            var languageId = Guid.Parse(id);
            var language = await _context.Languages.FindAsync(languageId);
            if (language is null) throw new NotFoundException<Core.Entities.Language>();

            language.LanguageName = dto.LanguageName;
            language.LanguageLevel = dto.LanguageLevel;

            await _context.SaveChangesAsync();
        }
    }
}