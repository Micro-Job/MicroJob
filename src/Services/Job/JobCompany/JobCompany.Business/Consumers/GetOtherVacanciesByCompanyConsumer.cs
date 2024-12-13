using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.VacancyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetOtherVacanciesByCompanyConsumer(JobCompanyDbContext dbContext) : IConsumer<GetOtherVacanciesByCompanyRequest>
{
    public async Task Consume(ConsumeContext<GetOtherVacanciesByCompanyRequest> context)
    {
        var vacancies = await dbContext.Vacancies
            .Where(x => x.CompanyId == context.Message.CompanyId && x.Id != context.Message.CurrentVacancyId)
            .OrderByDescending(x => x.StartDate)
            .Include(x => x.Company)
            .ToListAsync();

        if(vacancies is null)
        {
            await context.RespondAsync(new GetOtherVacanciesByCompanyResponse
            {
                Vacancies = []
            });

            return;
        }

        var response = new GetOtherVacanciesByCompanyResponse
        {
            Vacancies = vacancies.Select(x => new AllVacanyDto
            {
                CompanyName = x.Company.CompanyName,
                CompanyLogo = x.Company.CompanyLogo,
                Location = x.Company.CompanyLocation,
                Title = x.Title,
                WorkType = x.WorkType,
                IsActive = x.IsActive,
                IsVip = x.IsVip,
                ViewCount = x.ViewCount,
                MainSalary = x.MainSalary,
                CategoryId = x.CategoryId,
                StartDate = x.StartDate,
            }).ToList()
        };

        await context.RespondAsync(response);
    }
}
