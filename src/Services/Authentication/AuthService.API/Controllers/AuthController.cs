using AuthService.Business.Dtos;
using AuthService.Business.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Helpers;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
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

        [HttpGet("[action]")]
        public async Task<IActionResult> TryLanguage()
        {
            return Ok(MessageHelper.GetMessage("WELCOME"));
        }
    }
}