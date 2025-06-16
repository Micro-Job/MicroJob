using AuthService.Business.Dtos;
using Microsoft.AspNetCore.Mvc;
using AuthService.Business.Services.Auth;
using SharedLibrary.Helpers;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthenticationService _authService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CompanyRegister(RegisterCompanyDto dto)
        {
            await _authService.CompanyRegisterAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            return Ok(await _authService.LoginAsync(dto));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithRefreshToken(string refreshToken)
        {
            return Ok(await _authService.LoginWithRefreshTokenAsync(refreshToken));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            await _authService.ResetPasswordAsync(email);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword(PasswordResetDto resetDto)
        {
            await _authService.ConfirmPasswordResetAsync(resetDto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword)
        {
            await _authService.UpdatePasswordAsync(oldPassword, newPassword);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> TryLanguage()
        {
            return Ok(MessageHelper.GetMessage("WELCOME"));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckUserExist(string email , string phoneNumber)
        {
            await _authService.CheckUserExistAsync(email.Trim(), phoneNumber.Trim());
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyNameByVOEN(string voen)
        {
            return Ok(await _authService.GetCompanyNameByVOENAsync(voen));
        }
    }
}