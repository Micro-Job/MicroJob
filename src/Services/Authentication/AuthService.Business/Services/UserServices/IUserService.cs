using AuthService.Business.Dtos;

namespace AuthService.Business.Services.UserServices
{
    public interface IUserService
    {
        Task<UserInformationDto> GetUserInformationAsync();
        Task<UserUpdateResponseDto> UpdateUserInformation(UserUpdateDto dto);
        Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImage(UserProfileImageUpdateDto dto);
    }
}