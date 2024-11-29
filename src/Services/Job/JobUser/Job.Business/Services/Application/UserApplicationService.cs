using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Business.Services.CurrentUser;
using MassTransit;
using Shared.Events;

namespace Job.Business.Services.Application
{
    public class UserApplicationService : IUserApplicationService
    {
        readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;

        public UserApplicationService(IPublishEndpoint publishEndpoint, ICurrentUser currentUser)
        {
            _publishEndpoint = publishEndpoint;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
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