using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.UserDtos;

namespace Job.Business.Services.UserManagement;

public interface IUserManagementService
{
    Task<UserPersonalInfoDto> GetPersonalInfoAsync(string userId);
    Task<ResumeDetailItemDto> GetResumeDetailAsync(string userId);
}
