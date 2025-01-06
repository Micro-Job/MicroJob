using Shared.Dtos.ApplicationDtos;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        Task<ICollection<ApplicationDto>> GetUserApplicationsAsync(int skip, int take);
        Task CreateUserApplicationAsync(string vacancyId);
        Task<GetApplicationDetailResponse> GetUserApplicationByIdAsync(string applicationId);
    }
}
