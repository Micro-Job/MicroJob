using AuthService.Business.Dtos;

namespace AuthService.Business.Services.UserServices
{
    public interface IUserService
    {
        Task<UserInformationDto> GetUserInformationAsync();
        Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto);
        Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto);
    }
}