using Shared.Dtos.ApplicationDtos;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Responses;

namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        Task<ICollection<ApplicationDto>> GetUserApplicationsAsync(int skip, int take);
        Task CreateUserApplicationAsync(string vacancyId);
        Task<ApplicationDetailDto> GetUserApplicationByIdAsync(string id);
    }
}
