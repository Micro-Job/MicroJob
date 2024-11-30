using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

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