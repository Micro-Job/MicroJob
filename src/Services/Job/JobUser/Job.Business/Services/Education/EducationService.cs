using Job.Business.Dtos.EducationDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Education
{
    public class EducationService(JobDbContext context) : IEducationService
    {
        private readonly JobDbContext _context = context;

        public async Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos, Guid resumeId)
        {
            var educationsToAdd = dtos.Select(dto => new Core.Entities.Education
            {
                ResumeId = resumeId,
                InstitutionName = dto.InstitutionName,
                Profession = dto.Profession,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentEducation ? null : dto.EndDate,
                IsCurrentEducation = dto.IsCurrentEducation,
                ProfessionDegree = dto.ProfessionDegree
            }).ToList();

            await _context.Educations.AddRangeAsync(educationsToAdd);
            return educationsToAdd;
        }

        public async Task CreateEducationAsync(EducationCreateDto dto)
        {
            var education = new Core.Entities.Education
            {
                InstitutionName = dto.InstitutionName,
                Profession = dto.Profession,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentEducation ? null : dto.EndDate,
                IsCurrentEducation = dto.IsCurrentEducation,
                ProfessionDegree = dto.ProfessionDegree
            };
            await _context.Educations.AddAsync(education);
        }

        public async Task<ICollection<Core.Entities.Education>> UpdateBulkEducationAsync(ICollection<EducationUpdateDto> dtos, Guid resumeId)
        {
            var educationsToUpdate = new List<Core.Entities.Education>();

            foreach (var dto in dtos)
            {
                var education = await _context.Educations
                    .FirstOrDefaultAsync(e => e.ResumeId == resumeId && e.InstitutionName == dto.InstitutionName) ??
                    throw new NotFoundException<Core.Entities.Education>();

                education.InstitutionName = dto.InstitutionName;
                education.Profession = dto.Profession;
                education.StartDate = dto.StartDate;
                education.EndDate = dto.IsCurrentEducation ? null : dto.EndDate;
                education.IsCurrentEducation = dto.IsCurrentEducation;
                education.ProfessionDegree = dto.ProfessionDegree;

                educationsToUpdate.Add(education);
            }

            _context.Educations.UpdateRange(educationsToUpdate);
            await _context.SaveChangesAsync();

            return educationsToUpdate;
        }


        public async Task UpdateEducationAsync(EducationUpdateDto dto)
        {
            var education = await _context.Educations.FindAsync(dto.Id)
                ?? throw new NotFoundException<Core.Entities.Education>();
            education.InstitutionName = dto.InstitutionName;
            education.Profession = dto.Profession;
            education.StartDate = dto.StartDate;
            education.EndDate = dto.EndDate;
            education.IsCurrentEducation = dto.IsCurrentEducation;
            education.ProfessionDegree = dto.ProfessionDegree;
        }
    }
}