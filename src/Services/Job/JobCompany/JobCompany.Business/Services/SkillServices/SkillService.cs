using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Services.SkillServices;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.Skill
{
    public class SkillService(JobCompanyDbContext context) : ISkillService
    {
        private readonly JobCompanyDbContext _context = context;

        public async Task CreateSkillAsync(SkillDto dto)
        {
            var skill = new Core.Entites.Skill() { Name = dto.Name };
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GetAllSkillDto>> GetAllSkillsAsync()
        {
            var skills = await _context.Skills.Select(x=> new GetAllSkillDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return skills;
        }
    }
}
