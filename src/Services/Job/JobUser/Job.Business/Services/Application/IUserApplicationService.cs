using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        Task<GetUserApplicationsResponse> GetUserApplicationsAsync(int skip, int take);
        Task CreateUserApplicationAsync(string vacancyId);
    }
}