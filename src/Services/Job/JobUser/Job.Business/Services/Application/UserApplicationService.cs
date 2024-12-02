using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Shared.Events;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        //private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Guid userGuid;

        public UserApplicationService(IPublishEndpoint publishEndpoint,IHttpContextAccessor contextAccessor)
        {
            _publishEndpoint = publishEndpoint;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
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