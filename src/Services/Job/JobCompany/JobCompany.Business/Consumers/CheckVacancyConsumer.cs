using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class CheckVacancyConsumer : IConsumer<CheckVacancyRequest>
    {
        private readonly JobCompanyDbContext _companyDb;

        public CheckVacancyConsumer(JobCompanyDbContext companyDb)
        {
            _companyDb = companyDb;
        }

        public async Task Consume(ConsumeContext<CheckVacancyRequest> context)
        {
            var vacancyWithApplication = await _companyDb
                .Vacancies.Where(v => v.Id == context.Message.VacancyId)
                .Select(v => new
                {
                    Vacancy = v,
                    IsUserApplied = _companyDb.Applications.Any(a =>
                        a.VacancyId == v.Id && a.UserId == context.Message.UserId
                    ),
                })
                .FirstOrDefaultAsync();

            if (vacancyWithApplication == null)
            {
                await context.RespondAsync(
                    new CheckVacancyResponse
                    {
                        IsExist = false,
                        CompanyId = Guid.Empty,
                        VacancyName = string.Empty,
                        IsUserApplied = false,
                    }
                );
                return;
            }

            var response = new CheckVacancyResponse
            {
                IsExist = true,
                CompanyId = vacancyWithApplication.Vacancy.CompanyId ?? Guid.Empty,
                VacancyName = vacancyWithApplication.Vacancy.Title,
                IsUserApplied = vacancyWithApplication.IsUserApplied,
            };

            await context.RespondAsync(response);
        }
    }
}
