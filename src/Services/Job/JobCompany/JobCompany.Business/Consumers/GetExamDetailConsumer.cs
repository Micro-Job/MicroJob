using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;

namespace JobCompany.Business.Consumers
{
    public class GetExamDetailConsumer : IConsumer<GetExamDetailRequest>
    {
        private readonly JobCompanyDbContext _jobCompanyDbContext;

        public GetExamDetailConsumer(JobCompanyDbContext jobCompanyDbContext)
        {
            _jobCompanyDbContext = jobCompanyDbContext;
        }

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
                .FirstOrDefaultAsync();

            if (exam == null)
            {
                throw new NotFoundException<Exam>("Exam not found for the given vacancy.");
            }

            await context.RespondAsync(
                new GetExamDetailResponse
                {
                    ExamId = exam.ExamId.ToString(),
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
