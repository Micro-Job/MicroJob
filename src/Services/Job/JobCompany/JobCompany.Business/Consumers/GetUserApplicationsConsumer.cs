using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetUserApplicationsConsumer : IConsumer<GetUserApplicationsRequest>
{
    private readonly JobCompanyDbContext _jobCompanyDbContext;
    readonly IConfiguration _configuration;
    private readonly string? _authServiceBaseUrl;
    public GetUserApplicationsConsumer(JobCompanyDbContext jobCompanyDbContext, IConfiguration configuration)
    {
        _jobCompanyDbContext = jobCompanyDbContext;
        _configuration = configuration;
        _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
    }
    public async Task Consume(ConsumeContext<GetUserApplicationsRequest> context)
    {
        var userId = context.Message.UserId;

        var query = _jobCompanyDbContext.Applications
                           .Where(a => a.UserId == userId)
                           .Include(a => a.Vacancy)
                           .ThenInclude(a => a.Company)
                           .Include(a => a.Status);

        var applications = await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
            .Take(context.Message.Take)
            .ToListAsync();

        if (!applications.Any())
        {
            await context.RespondAsync(new GetUserApplicationsResponse()
            {
                UserApplications = []
            });
            return;
        }

        var response = new GetUserApplicationsResponse
        {
            UserApplications = applications.Select(a => new ApplicationDto
            {
                VacancyId = a.VacancyId,
                title = a.Vacancy.Title,
                CompanyName = a.Vacancy.CompanyName,
                companyLogo = $"{_authServiceBaseUrl}/{a.Vacancy.Company.CompanyLogo}",
                CompanyId = a.Vacancy.CompanyId,
                WorkType = Enum.GetName(typeof(WorkType), a.Vacancy.WorkType),
                IsActive = a.Vacancy.IsActive,
                StatusName = a.Status?.StatusName,
                StatusColor = a.Status?.StatusColor,
                ViewCount = a.Vacancy.ViewCount,
                startDate = a.CreatedDate
            }).ToList()
        };

        await context.RespondAsync(response);
    }
}
