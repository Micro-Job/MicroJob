using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetApplicationDetailConsumer(JobCompanyDbContext _context,IConfiguration _configuration) : IConsumer<GetApplicationDetailRequest>
    {
        private readonly string? _authServiceBaseUrl = _configuration["AuthService:BaseUrl"];

        public async Task Consume(ConsumeContext<GetApplicationDetailRequest> context)
        {
            var guidVacId = Guid.Parse(context.Message.ApplicationId);

            var responseMessage = await _context
                .Applications.Include(a => a.Vacancy)
                .ThenInclude(v => v.Company)
                .ThenInclude(c => c.Statuses)
                .Include(a => a.Status)
                .Where(a => a.Id == guidVacId).Select(x=> new GetApplicationDetailResponse
                {
                    VacancyId = x
                    .Vacancy.Id.ToString(),
                    VacancyName = x.Vacancy.Title,
                    CompanyName = x.Vacancy.CompanyName,
                    CompanyLogo = $"{_authServiceBaseUrl}/{x.Vacancy.Company.CompanyLogo}",
                    Location = x.Vacancy.Location,
                    WorkType = x.Vacancy.WorkType,
                    WorkStyle = x.Vacancy.WorkStyle,
                    startDate = x.CreatedDate,
                    CompanyStatuses = x.Vacancy.Company.Statuses != null ? x.Vacancy.Company.Statuses.Select(cs => cs.StatusName).ToList() : new List<string>(),
                    ApplicationStatus = x.Status.StatusName,
                    IsSaved = false,
                }).FirstOrDefaultAsync();

            if (responseMessage == null)
            {
                await context.RespondAsync<GetApplicationDetailResponse>(null);
                return;
            }

            await context.RespondAsync(responseMessage);
        }
    }
}