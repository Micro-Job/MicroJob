using Job.Business.Exceptions.Common;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Shared.Events;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Security.Claims;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<GetUserApplicationsRequest> _userApplicationRequest;
        private readonly IRequestClient<CheckVacancyRequest> _requestClient;
        private readonly Guid userGuid;

        public UserApplicationService(IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor, IRequestClient<GetUserApplicationsRequest> userApplicationRequest, IRequestClient<CheckVacancyRequest> requestClient)
        {
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            _requestClient = requestClient;
            _userApplicationRequest = userApplicationRequest;
            userGuid = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
        }

        /// <summary> İstifadəçinin bütün müraciətlərini gətirir </summary>
        public async Task<GetUserApplicationsResponse> GetUserApplicationsAsync(int skip, int take)
        {
            GetUserApplicationsRequest request = new() { UserId = userGuid, Skip = skip, Take = take };

            var response = await _userApplicationRequest.GetResponse<GetUserApplicationsResponse>(request);

            return response.Message;
        }

        /// <summary> Eventle userin application yaratmasi  ve 
        /// vakansiyaya muraciet ederken companye bildiris getmesi </summary>
        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var guidVac = Guid.Parse(vacancyId);

            var response = await _requestClient.GetResponse<CheckVacancyResponse>(new CheckVacancyRequest
            {
                VacancyId = guidVac
            });

            if (!response.Message.IsExist) throw new EntityNotFoundException("Vacancy");
            var companyId = response.Message.CompanyId;

            await _publishEndpoint.Publish(new UserApplicationEvent
            {
                UserId = userGuid,
                VacancyId = guidVac,
                CreatedDate = DateTime.Now,
            });

            await _publishEndpoint.Publish(new VacancyApplicationEvent
            {
                UserId = companyId,
                SenderId = userGuid,
                VacancyId = guidVac,
                Content = $"İstifadəçi {userGuid} {guidVac} vakansiyasına müraciət etdi.",
            });
        }
    }
}
