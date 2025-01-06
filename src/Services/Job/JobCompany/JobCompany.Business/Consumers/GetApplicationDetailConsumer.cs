using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Requests;
using Shared.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetApplicationDetailConsumer : IConsumer<GetApplicationDetailRequest>
    {
        private readonly JobCompanyDbContext _jobCompanyDbContext;
        private readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public GetApplicationDetailConsumer(
            JobCompanyDbContext jobCompanyDbContext,
            IConfiguration configuration
        )
        {
            _jobCompanyDbContext = jobCompanyDbContext;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        public async Task Consume(ConsumeContext<GetApplicationDetailRequest> context)
        {
            var guidVacId = Guid.Parse(context.Message.ApplicationId);
            var application = await _jobCompanyDbContext
                .Applications.Include(a => a.Vacancy)
                .ThenInclude(v => v.Company)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == guidVacId);

            if (application == null)
            {
                await context.RespondAsync<GetApplicationDetailResponse>(null);
                return;
            }

            var response = new GetApplicationDetailResponse
            {
                VacancyName = application.Vacancy.Title,
                CompanyName = application.Vacancy.CompanyName,
                CompanyLogo = $"{_authServiceBaseUrl}/{application.Vacancy.Company.CompanyLogo}",
                Location = application.Vacancy.Location,
                WorkType = application.Vacancy.WorkType,
                WorkStyle = application.Vacancy.WorkStyle,
                startDate = application.CreatedDate,
                CompanyStatuses =
                    application.Vacancy?.Company?.Statuses != null
                        ? application.Vacancy.Company.Statuses.Select(cs => cs.StatusName).ToList()
                        : new List<string>(),
                ApplicationStatus = application.Status?.StatusName,
            };

            await context.RespondAsync(response);
        }
    }
}
