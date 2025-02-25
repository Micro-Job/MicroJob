using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Skill
{
    public class SkillService(JobDbContext _context) : ISkillService
    {
        public async Task CreateSkillAsync(SkillDto dto)
        {
            //TODO : bu hisse trim ve tolower olmadan olmalidir
            //bool isExist = await _context.Skills.AnyAsync(x => x.Name.Trim().ToLower() == dto.Name.Trim().ToLower());
            
            //if (isExist) throw new IsAlreadyExistException<Core.Entities.Skill>();

            var skill = new Core.Entities.Skill()
            {
                Name = dto.Name.Trim(),
            };
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GetAllSkillDto>> GetAllSkillsAsync()
        {
            var skills = await _context.Skills.Select(x=> new GetAllSkillDto
            {
                Name = x.Name,
            }).ToListAsync();

            return skills;
        }
    }
}