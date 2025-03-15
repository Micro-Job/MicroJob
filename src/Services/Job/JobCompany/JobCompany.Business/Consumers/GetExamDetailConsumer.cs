using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;

namespace JobCompany.Business.Consumers
{
    public class GetExamDetailConsumer(JobCompanyDbContext jobCompanyDbContext) : IConsumer<GetExamDetailRequest>
    {
        private readonly JobCompanyDbContext _jobCompanyDbContext = jobCompanyDbContext;

        public async Task Consume(ConsumeContext<GetExamDetailRequest> context)
        {
            var guidVacId = Guid.Parse(context.Message.VacancyId);
            var exam = await _jobCompanyDbContext
                .Vacancies.Where(v => v.Id == guidVacId)
                .Select(e => new
                {
                    e.ExamId,
                    e.Company.CompanyName,
                    e.Exam.ExamQuestions.Count,
                    e.Exam.Duration,
                    e.Exam.IntroDescription,
                    e.Exam.LimitRate,
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Exam>("Exam not found for the given vacancy.");
            await context.RespondAsync(
                new GetExamIntroDto
                {
                    CompanyName = exam.CompanyName,
                    QuestionCount = exam.Count,
                    Duration = exam.Duration,
                    IntroDescription = exam.IntroDescription,
                    LimitRate = exam.LimitRate,
                }
            );
        }
    }
}