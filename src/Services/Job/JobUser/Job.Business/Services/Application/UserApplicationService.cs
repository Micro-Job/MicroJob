using MassTransit;
using Microsoft.AspNetCore.Http;
using Shared.Events;
using System.Security.Claims;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Guid userGuid;

        public UserApplicationService(IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor)
        {
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            userGuid = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
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