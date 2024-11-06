using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Services.User
{
    public class UserInformationService(IRequestClient<GetUserDataRequest> _client) : IUserInformationService
    {
        public async Task<GetUserDataResponse> GetUserDataAsync(Guid userId)
        {
            var response = await _client.GetResponse<GetUserDataResponse>(new GetUserDataRequest { UserId = userId });
            return response.Message;
        }
    }
}
