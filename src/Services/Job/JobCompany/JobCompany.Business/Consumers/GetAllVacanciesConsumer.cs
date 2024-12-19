using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.CompanyDtos;
using Shared.Dtos.VacancyDtos;
using Shared.Requests;
using Shared.Responses;

public class GetAllVacanciesConsumer : IConsumer<GetAllVacanciesRequest>
{
    private readonly JobCompanyDbContext _context;
    readonly IConfiguration _configuration;
    private readonly string? _authServiceBaseUrl;

    public GetAllVacanciesConsumer(JobCompanyDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
    }

    public async Task Consume(ConsumeContext<GetAllVacanciesRequest> context)
    {
        var request = context.Message;

        var query = _context.Vacancies
            .Include(v => v.Company)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.TitleName))
        {
            query = query.Where(v => EF.Functions.Like(v.Title, $"%{request.TitleName}%"));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(v => v.IsActive == request.IsActive);
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

        if (request.MinSalary.HasValue)
        {
            query = query.Where(v => v.MainSalary >= request.MinSalary);
        }

        if (request.MaxSalary.HasValue)
        {
            query = query.Where(v => v.MainSalary <= request.MaxSalary);
        }

        var vacancies = await query
            // .Include(q => q.Skills)
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
                IsVip = v.IsVip,
                CategoryId = v.CategoryId,
                Company = new CompanyDetailDto
                {
                    CompanyId = v.Company.Id,
                    CityId = v.Company.CityId,
                    CountryId = v.Company.CountryId,
                }
            })
            .ToListAsync();

        await context.RespondAsync(new GetAllVacanciesResponse
        {
            Vacancies = vacancies
        });
    }
}
