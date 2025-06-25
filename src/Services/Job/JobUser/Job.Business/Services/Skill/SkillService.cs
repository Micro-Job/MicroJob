using Job.Business.Dtos.SkillDtos;
using Job.Business.Extensions;
using Job.Business.Statistics;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace Job.Business.Services.Skill;

public class SkillService(JobDbContext _context, ICurrentUser _user) 
{
    public async Task CreateSkillAsync(SkillCreateDto dto)
    {
        Core.Entities.Skill skill = new()
        {
            Translations = dto.Skills.Select(x => new SkillTranslation
            {
                Language = x.Language,
                Name = x.Name.Trim()
            }).ToList()
        };
        await _context.Skills.AddAsync(skill);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GetAllSkillDto>> GetAllSkillsAsync()
    {
        var skills = await _context.Skills.Include(x=> x.Translations)
            .AsNoTracking()
            .Select(b => new GetAllSkillDto
            {
                Id = b.Id,
                Name = b.Translations.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Name),
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