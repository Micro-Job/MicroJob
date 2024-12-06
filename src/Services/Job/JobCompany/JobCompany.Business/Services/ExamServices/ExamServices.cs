using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;

namespace JobCompany.Business.Services.ExamServices
{
    public class ExamServices(JobCompanyDbContext context) : IExamServices
    {
        private readonly JobCompanyDbContext _context = context;
        public Task CreateExamAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                TemplateId = dto.TemplateId,
                IntroDescription = dto.IntroDescription,
                LastDescription = dto.LastDescription,
                Result = dto.Result,
            }
            throw new NotImplementedException();
        }
    }
}