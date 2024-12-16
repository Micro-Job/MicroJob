using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Skill
{
    public class SkillService(JobDbContext context) : ISkillService
    {
        private readonly JobDbContext _context = context;

        public async Task CreateSkillAsync(SkillDto dto)
        {
            bool isExist = await _context.Skills.AnyAsync(x => x.Name.Trim().ToLower() == dto.Name.Trim().ToLower());
            
            if (isExist) throw new IsAlreadyExistException<Core.Entities.Skill>();

            var skill = new Core.Entities.Skill()
            {
                Name = dto.Name,
            };
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GetAllSkillDto>> GetAllSkillsAsync()
        {
            var skills = await _context.Skills.ToListAsync();
            return skills.Select(s => new GetAllSkillDto
            {
                Name = s.Name
            }).ToList();
        }
    }
}