using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetUserApplicationsConsumer(JobCompanyDbContext jobCompanyDbContext, IConfiguration configuration) : IConsumer<GetUserApplicationsRequest>
{
    private readonly JobCompanyDbContext _jobCompanyDbContext = jobCompanyDbContext ?? throw new ArgumentNullException(nameof(jobCompanyDbContext));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly string _authServiceBaseUrl = configuration["AuthService:BaseUrl"] ?? string.Empty;

    public async Task Consume(ConsumeContext<GetUserApplicationsRequest> context)
    {
        var userId = context.Message.UserId;

        var query = _jobCompanyDbContext.Applications
            .Where(a => a.UserId == userId)
            .Include(a => a.Vacancy)
            .ThenInclude(v => v.Company)
            .Include(a => a.Status);

        var totalCount = await query.CountAsync();

        int skipValue = (context.Message.Skip - 1) * context.Message.Take;
        if (skipValue >= totalCount) skipValue = 0;

        var applications = await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip(skipValue)
            .Take(context.Message.Take)
            .ToListAsync();

        if (applications.Count == 0)
        {
            await context.RespondAsync(new GetUserApplicationsResponse
            {
                UserApplications = [],
                TotalCount = totalCount
            });
            return;
        }

        var response = new GetUserApplicationsResponse
        {
            UserApplications = applications.Select(a => new ApplicationDto
            {
                ApplicationId = a.Id,
                VacancyId = a.VacancyId,
                title = a.Vacancy.Title ?? "No Title",
                CompanyName = a.Vacancy.Company?.CompanyName ?? "No Company",
                companyLogo = string.IsNullOrEmpty(a.Vacancy.Company?.CompanyLogo)
                    ? $"{_authServiceBaseUrl}/default-logo.png"
                    : $"{_authServiceBaseUrl}/{a.Vacancy.Company.CompanyLogo}",
                CompanyId = a.Vacancy.CompanyId,
                WorkType = a.Vacancy.WorkType.HasValue ? Enum.GetName(typeof(WorkType), a.Vacancy.WorkType) : "Unknown",
                IsActive = a.Vacancy.IsActive,
                //StatusName = a.Status?.Name ?? "Pending",
                StatusColor = a.Status?.StatusColor ?? "#CCCCCC",
                ViewCount = a.Vacancy.ViewCount,
                startDate = a.CreatedDate
            }).ToList(),
            TotalCount = totalCount
        };

        await context.RespondAsync(response);
    }
}