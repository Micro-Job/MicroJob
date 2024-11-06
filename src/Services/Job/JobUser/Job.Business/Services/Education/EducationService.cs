using Job.Business.Dtos.EducationDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;

namespace Job.Business.Services.Education
{
    public class EducationService : IEducationService
    {
        private readonly JobDbContext _context;

        public EducationService(JobDbContext context)
        {
            _context = context;
        }
            public async Task<ICollection<Core.Entities.Education>> CreateBulkEducationAsync(ICollection<EducationCreateDto> dtos)
            {
                var educationsToAdd = dtos.Select(dto => new Core.Entities.Education
                {
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
        }

        public async Task UpdateEducationAsync(string id, EducationUpdateDto dto)
        {
            var educationId = Guid.Parse(id);
            var education = await _context.Educations.FindAsync(educationId);

            if (education is null) throw new NotFoundException<Core.Entities.Education>();

            education.InstitutionName = dto.InstitutionName;
            education.Profession = dto.Profession;
            education.StartDate = dto.StartDate;
            education.EndDate = dto.EndDate;
            education.IsCurrentEducation = dto.IsCurrentEducation;
            education.ProfessionDegree = dto.ProfessionDegree;
        }
    }
}