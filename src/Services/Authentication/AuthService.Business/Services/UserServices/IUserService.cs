using AuthService.Business.Dtos;
using AuthService.Core.Enums;

namespace AuthService.Business.Services.UserServices
{
    public interface IUserService
    {
        Task<UserInformationDto> GetUserInformationAsync();
        Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto);
        Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto);
        Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto);
    }
}