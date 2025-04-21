using AuthService.Business.Dtos;
using SharedLibrary.Enums;

namespace AuthService.Business.Services.UserServices
{
    public interface IUserService
    {
        Task<UserInformationDto> GetUserInformationAsync();
        Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto);
        Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto);
        Task<DataListDto<BasicUserInfoDto>> GetAllUsersAsync(UserRole userRole, string? searchTerm, int pageIndex = 1, int pageSize = 10);
    }
}