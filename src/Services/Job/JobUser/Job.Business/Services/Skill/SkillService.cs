using Job.Business.Dtos.SkillDtos;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Skill
{
    public class SkillService(JobDbContext context) : ISkillService
    {
        private readonly JobDbContext _context = context;

        public async Task CreateSkillAsync(SkillDto dto)
        {
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