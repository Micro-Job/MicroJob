using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;

namespace Job.Business.Services.Experience
{
    public class ExperienceService(JobDbContext context) : IExperienceService
    {
        private readonly JobDbContext _context = context;

        public async Task<ICollection<Core.Entities.Experience>> CreateBulkExperienceAsync(ICollection<ExperienceCreateDto> dtos,Guid resumeId)
        {
            var experiencesToAdd = dtos.Select(dto => new Core.Entities.Experience
            {
                ResumeId = resumeId,
                OrganizationName = dto.OrganizationName,
                PositionName = dto.PositionName,
                PositionDescription = dto.PositionDescription,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentOrganization ? null : dto.EndDate,
                IsCurrentOrganization = dto.IsCurrentOrganization
            }).ToList();

            await _context.Experiences.AddRangeAsync(experiencesToAdd);
            await _context.SaveChangesAsync();

            return experiencesToAdd;
        }

        public async Task CreateExperienceAsync(ExperienceCreateDto dto)
        {
            var experience = new Core.Entities.Experience
            {
                OrganizationName = dto.OrganizationName,
                PositionName = dto.PositionName,
                PositionDescription = dto.PositionDescription,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentOrganization ? null : dto.EndDate,
                IsCurrentOrganization = dto.IsCurrentOrganization
            };
            await _context.Experiences.AddAsync(experience);
        }

        public async Task UpdateExperienceAsync(string id, ExperienceUpdateDto dto)
        {
            var resumeId = Guid.Parse(id);
            var experience = await _context.Experiences.FindAsync(resumeId)
                ?? throw new NotFoundException<Core.Entities.Experience>();
            experience.OrganizationName = dto.OrganizationName;
            experience.PositionName = dto.PositionName;
            experience.PositionDescription = dto.PositionDescription;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.IsCurrentOrganization ? null : dto.EndDate;
            experience.IsCurrentOrganization = dto.IsCurrentOrganization;
        }
    }
}