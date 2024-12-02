using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Shared.Events;

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

        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var guidVac = Guid.Parse(vacancyId);
            await _publishEndpoint.Publish(new UserApplicationEvent
            {
                UserId = userGuid,
                VacancyId = guidVac
            });
        }
    }
}