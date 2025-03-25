using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Enums;

namespace JobCompany.Business.Consumers;
public class GetAllVacanciesConsumer(JobCompanyDbContext context, IConfiguration configuration) : IConsumer<GetAllVacanciesRequest>
{
    private readonly JobCompanyDbContext _context = context;
    readonly IConfiguration _configuration = configuration;
    private readonly string? _authServiceBaseUrl = configuration["AuthService:BaseUrl"];

    public async Task Consume(ConsumeContext<GetAllVacanciesRequest> context)
    {
        var request = context.Message;

        var query = _context.Vacancies.Include(v => v.Company).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.TitleName))
        {
            query = query.Where(v => v.Title.ToLower().Contains(request.TitleName.ToLower()));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(v => v.VacancyStatus == request.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryId))
        {
            var categoryId = Guid.Parse(request.CategoryId);
            query = query.Where(v => v.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.CountryId))
        {
            var countryId = Guid.Parse(request.CountryId);
            query = query.Where(v => v.Company.CountryId == countryId);
        }

        if (!string.IsNullOrWhiteSpace(request.CityId))
        {
            var cityId = Guid.Parse(request.CityId);
            query = query.Where(v => v.Company.CityId == cityId);
        }

        if (!string.IsNullOrWhiteSpace(request.CompanyId))
        {
            var companyId = Guid.Parse(request.CompanyId);
            query = query.Where(x => x.Company.Id == companyId);
        }

        if (request.WorkType.HasValue && request.WorkType.Value != 0 && Enum.IsDefined(typeof(WorkType), request.WorkType.Value))
        {
            query = query.Where(x => x.WorkType == request.WorkType);
        }

        if (request.WorkStyle.HasValue && request.WorkStyle.Value != 0 && Enum.IsDefined(typeof(WorkStyle), request.WorkStyle.Value))
        {
            query = query.Where(x => x.WorkStyle == request.WorkStyle);
        }

        if (request.MinSalary.HasValue)
        {
            query = query.Where(v => v.MainSalary >= request.MinSalary);
        }

        if (request.MaxSalary.HasValue)
        {
            query = query.Where(v => v.MainSalary <= request.MaxSalary);
        }

        var vacancies = await query
            .Select(v => new AllVacanyDto
            {
                VacancyId = v.Id.ToString(),
                Title = v.Title,
                CompanyName = v.Company.CompanyName,
                CompanyLogo = $"{_authServiceBaseUrl}/{v.Company.CompanyLogo}",
                StartDate = v.StartDate,
                Location = v.Location,
                ViewCount = v.ViewCount,
                MainSalary = v.MainSalary,
                WorkType = v.WorkType,
                WorkStyle = v.WorkStyle,
                IsVip = v.IsVip,
                IsActive = v.VacancyStatus,
                CategoryId = v.CategoryId,
            })
            .ToListAsync();

        await context.RespondAsync(new GetAllVacanciesResponse { Vacancies = vacancies, TotalCount = vacancies.Count });
    }
}