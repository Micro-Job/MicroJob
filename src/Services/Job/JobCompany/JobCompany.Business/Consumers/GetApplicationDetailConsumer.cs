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
    public class GetApplicationDetailConsumer(
        JobCompanyDbContext jobCompanyDbContext,
        IConfiguration configuration,
        IRequestClient<IsApplicationSavedRequest> requestClient
        ) : IConsumer<GetApplicationDetailRequest>
    {
        private readonly JobCompanyDbContext _jobCompanyDbContext = jobCompanyDbContext;
        private readonly IConfiguration _configuration = configuration;
        private readonly string? _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        private readonly IRequestClient<IsApplicationSavedRequest> _isApplicationSavedClient = requestClient;

        public async Task Consume(ConsumeContext<GetApplicationDetailRequest> context)
        {
            var guidVacId = Guid.Parse(context.Message.ApplicationId);
            var application = await _jobCompanyDbContext
                .Applications.Include(a => a.Vacancy)
                .ThenInclude(v => v.Company)
                .ThenInclude(c => c.Statuses)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == guidVacId) ?? throw new NotFoundException<Application>();

            var vacancyId = application.VacancyId.ToString();

            if (application == null)
            {
                await context.RespondAsync<GetApplicationDetailResponse>(null);
                return;
            }

            var userGuid = string.IsNullOrEmpty(context.Message.UserId) ? (Guid?)null : Guid.Parse(context.Message.UserId);
            var isSaved = false;

            if (userGuid != null)
            {
                var response = await _isApplicationSavedClient.GetResponse<IsApplicationSavedResponse>(
                    new IsApplicationSavedRequest { UserId = userGuid.ToString(), VacancyId = vacancyId }
                );

                isSaved = response.Message.IsSaved;
            }

            var responseMessage = new GetApplicationDetailResponse
            {
                VacancyId = application.Vacancy.Id.ToString(),
                VacancyName = application.Vacancy.Title,
                CompanyName = application.Vacancy.CompanyName,
                CompanyLogo = $"{_authServiceBaseUrl}/{application.Vacancy.Company.CompanyLogo}",
                Location = application.Vacancy.Location,
                WorkType = application.Vacancy.WorkType,
                WorkStyle = application.Vacancy.WorkStyle,
                startDate = application.CreatedDate,
                CompanyStatuses =
            application.Vacancy?.Company?.Statuses != null
                ? application.Vacancy.Company.Statuses.Select(cs => cs.StatusName).ToList() : [],
                ApplicationStatus = application.Status?.StatusName,
                IsSaved = isSaved,
            };

            await context.RespondAsync(responseMessage);
        }
    }
}