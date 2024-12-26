using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetOtherVacanciesByCompanyConsumer : IConsumer<GetOtherVacanciesByCompanyRequest>
{
    private readonly JobCompanyDbContext _dbContext;
    readonly IConfiguration _configuration;
    private readonly string? _authServiceBaseUrl;

    public GetOtherVacanciesByCompanyConsumer(JobCompanyDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
    }

    public async Task Consume(ConsumeContext<GetOtherVacanciesByCompanyRequest> context)
    {
        var vacanciesQuery = _dbContext.Vacancies
            .Where(x => x.CompanyId == context.Message.CompanyId);

        if (context.Message.CurrentVacancyId.HasValue)
        {
            vacanciesQuery = vacanciesQuery.Where(x => x.Id != context.Message.CurrentVacancyId);
        }

        var vacancies = await vacanciesQuery
            .OrderByDescending(x => x.StartDate)
            .Include(x => x.Company)
            .AsNoTracking()
            .Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
            .Take(context.Message.Take)
            .ToListAsync();

        if (vacancies is null || !vacancies.Any())
        {
            await context.RespondAsync(new GetOtherVacanciesByCompanyResponse
            {
                Vacancies = new List<AllVacanyDto>()
            });
            return;
        }

        var response = new GetOtherVacanciesByCompanyResponse
        {
            Vacancies = vacancies.Select(x => new AllVacanyDto
            {
                VacancyId = x.Id.ToString(),
                CompanyName = x.CompanyName,
                CompanyLogo = $"{_authServiceBaseUrl}/{x.Company.CompanyLogo}",
                Location = x.Location,
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
