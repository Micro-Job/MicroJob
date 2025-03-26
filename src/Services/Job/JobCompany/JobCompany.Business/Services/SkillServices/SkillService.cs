 using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Extensions;
using JobCompany.Business.Services.SkillServices;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.Skill
{
    public class SkillService(JobCompanyDbContext context, ICurrentUser _user) : ISkillService
    {
        private readonly JobCompanyDbContext _context = context;

        public async Task CreateSkillAsync(SkillCreateDto dto)
        {
            Core.Entites.Skill skill = new();
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();

            var skillTranslations = dto.Skills.Select(x => new SkillTranslation
            {
                SkillId = skill.Id,
                Language = x.Language,
                Name = x.Name.Trim()
            }).ToList();

            await _context.SkillTranslations.AddRangeAsync(skillTranslations);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GetAllSkillDto>> GetAllSkillsAsync()
        {
            var skills = await _context.Skills
                .IncludeTranslations()
            .Select(b => new GetAllSkillDto
            {
                Id = b.Id,
                Name = b.GetTranslation(_user.LanguageCode,GetTranslationPropertyName.Name),
            })
            .ToListAsync();

            return skills;
        }

        public async Task UpdateSkillAsync(List<SkillUpdateDto> dtos)
        {
            var skillTranslations = await _context.SkillTranslations
            .Where(x => dtos.Select(b => b.Id).Contains(x.Id))
            .ToListAsync();

            foreach (var translation in skillTranslations)
            {
                var skill = dtos.FirstOrDefault(b => b.Id == translation.Id);
                if (skill != null)
                {
                    translation.Name = skill.Name;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSkillAsync(string skillId)
        {
            var skillGuid = Guid.Parse(skillId);
            var skill = await _context.Skills.Include(x => x.Translations).Where(x => x.Id == skillGuid).FirstOrDefaultAsync();

            var skillTranslations = skill.Translations.Select(x => x).ToList();
            _context.SkillTranslations.RemoveRange(skillTranslations);
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }
}
