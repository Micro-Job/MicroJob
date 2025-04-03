using AuthService.Business.Dtos;

namespace AuthService.Business.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task CompanyRegisterAsync(RegisterCompanyDto dto);

        Task<TokenResponseDto> LoginAsync(LoginDto dto);

        Task<TokenResponseDto> LoginWithRefreshTokenAsync(string refreshToken);

        Task ResetPasswordAsync(string email);

        Task ConfirmPasswordResetAsync(PasswordResetDto dto);

        Task UpdatePasswordAsync(string oldPassword, string newPassword);
    }
}