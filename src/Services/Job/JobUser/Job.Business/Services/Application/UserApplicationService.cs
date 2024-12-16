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
        private readonly Guid userGuid;

        public UserApplicationService(IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor, IRequestClient<GetUserApplicationsRequest> userApplicationRequest)
        {
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
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
            await _publishEndpoint.Publish(new UserApplicationEvent
            {
                UserId = userGuid,
                VacancyId = guidVac
            });

            await _publishEndpoint.Publish(new VacancyApplicationEvent
            {
                UserId = userGuid,
                VacancyId = guidVac,
                Content = $"İstifadəçi {userGuid} {guidVac} vakansiyasına müraciət etdi."
            });
        }
    }
}