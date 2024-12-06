using JobCompany.Business.Dtos.ExamDtos.AnswerDtos;
using JobCompany.DAL.Contexts;

namespace JobCompany.Business.Services.ExamServices.AnswerServices
{
    public class AnswerService(JobCompanyDbContext context) : IAnswerService
    {
        private readonly JobCompanyDbContext _context = context;
        public async Task CreateAnswerAsync(CreateAnswerDto dto)
        {
            var answer = await _context.
        }
    }
}