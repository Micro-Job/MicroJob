using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class CheckVacancyConsumer(JobCompanyDbContext companyDb) : IConsumer<CheckVacancyRequest>
{
    public async Task Consume(ConsumeContext<CheckVacancyRequest> context)
    {
        var vacancy = await companyDb.Vacancies.FirstOrDefaultAsync(v => v.Id == context.Message.VacancyId);
        
        var response = new CheckVacancyResponse
        {
            IsExist = vacancy != null,
            CompanyId = vacancy?.CompanyId ?? Guid.Empty,
            VacancyName = vacancy?.Title ?? string.Empty
        };
        
        await context.RespondAsync(response);
    }
}