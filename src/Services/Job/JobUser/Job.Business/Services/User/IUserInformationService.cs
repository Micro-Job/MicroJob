using Job.Business.Dtos.UserDtos;
using SharedLibrary.Enums;
using SharedLibrary.Responses;

namespace Job.Business.Services.User
{
    public interface IUserInformationService
    {
        Task<GetUserDataResponse> GetUserDataAsync(Guid userId);
        Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto);
    }
}