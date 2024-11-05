using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.EducationDtos;
using Job.Core.Entities;
using Job.DAL.Contexts;

namespace Job.Business.Services.Education
{
    public class EducationService : IEducationService
    {
        private readonly AppDbContext _context;

        public EducationService(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(EducationCreateDto dto)
        {
            var resumeId = Guid.Parse(dto.ResumeId);
            var education = new Core.Entities.Education
            {
                ResumeId = resumeId,
                InstitutionName = dto.InstitutionName,
                Profession = dto.Profession,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentEducation ? null : dto.EndDate,
                IsCurrentEducation = dto.IsCurrentEducation,
                ProfessionDegree = dto.ProfessionDegree
            };

            await _context.Educations.AddAsync(education);
            await _context.SaveChangesAsync();
        }
    }
}