using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Exceptions.Common;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Experience
{
    public class ExperienceService : IExperienceService
    {
        private readonly JobDbContext _context;

        public ExperienceService(JobDbContext context)
        {
            _context = context;
        }

        public async Task CreateExperienceAsync(ExperienceCreateDto dto)
        {
            var resumeId = Guid.Parse(dto.ResumeId);
            var resume = await _context.Resumes.FindAsync(resumeId);
            if (resume == null) throw new NotFoundException<Core.Entities.Experience>();

            var experience = new Core.Entities.Experience
            {
                ResumeId = resume.Id,
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
            var experience = await _context.Experiences.FindAsync(resumeId);
            if (experience is null) throw new NotFoundException<Core.Entities.Experience>();

            experience.OrganizationName = dto.OrganizationName;
            experience.PositionName = dto.PositionName;
            experience.PositionDescription = dto.PositionDescription;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.IsCurrentOrganization ? null : dto.EndDate;
            experience.IsCurrentOrganization = dto.IsCurrentOrganization;

            await _context.SaveChangesAsync();
        }
    }
}
