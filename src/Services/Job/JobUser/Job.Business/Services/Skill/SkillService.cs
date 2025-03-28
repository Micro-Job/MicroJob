using Job.Business.Dtos.SkillDtos;
using Job.Business.Extensions;
using Job.Business.Statistics;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace Job.Business.Services.Skill
{
    public class SkillService(JobDbContext _context, ICurrentUser _user) : ISkillService
    {
        public async Task CreateSkillAsync(SkillCreateDto dto)
        {
            Core.Entities.Skill skill = new();
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
    }
}