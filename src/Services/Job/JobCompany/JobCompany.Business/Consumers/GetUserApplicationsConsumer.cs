using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers;

public class GetUserApplicationsConsumer(JobCompanyDbContext jobCompanyDbContext) : IConsumer<GetUserApplicationsRequest>
{
    public async Task Consume(ConsumeContext<GetUserApplicationsRequest> context)
    {
        var userId = context.Message.UserId;

        var query = jobCompanyDbContext.Applications
                           .Where(a => a.UserId == userId)
                           .Include(a => a.Vacancy)
                           .Include(a => a.Status);

        var applications = await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip(context.Message.Skip)
            .Take(context.Message.Take)
            .ToListAsync();

        if (applications == null)
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
                VacancyTitle = a.Vacancy.Title,
                CompanyName = a.Vacancy.CompanyName,
                CompanyId = a.Vacancy.CompanyId,
                WorkType = Enum.GetName(typeof(WorkType), a.Vacancy.WorkType),
                IsActive = a.Vacancy.IsActive,
                StatusName = a.Status?.StatusName,
                StatusColor = a.Status?.StatusColor,
                ViewCount = a.Vacancy.ViewCount,
                CreatedDate = a.CreatedDate
            }).ToList()
        };

        await context.RespondAsync(response);
    }
}
