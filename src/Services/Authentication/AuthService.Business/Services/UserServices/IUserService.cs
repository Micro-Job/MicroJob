using AuthService.Business.Dtos;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SharedLibrary.Responses;

namespace AuthService.Business.Services.UserServices
{
    public interface IUserService
    {
        Task<UserInformationDto> GetUserInformationAsync();
        Task<UserUpdateResponseDto> UpdateUserInformationAsync(UserUpdateDto dto);
        Task<UserProfileImageUpdateResponseDto> UpdateUserProfileImageAsync(UserProfileImageUpdateDto dto);
    }
}