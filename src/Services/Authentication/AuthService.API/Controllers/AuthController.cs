using AuthService.Business.Dtos;
using AuthService.Business.Services.Auth;
using AuthService.Business.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
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

        [HttpGet("user-info")]

        public async Task<IActionResult> GetUserInfoAsync()
        {
            var data = await _userService.GetUserInformationAsync();
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserUpdateDto dto)
        {
            await _userService.UpdateUserInformation(dto);
            return Ok();
        }

        [HttpPut("image")]
        public async Task<IActionResult> Update(UserProfileImageUpdateDto dto)
        {
            await _userService.UpdateUserProfileImage(dto);
            return Ok();
        }
    }
}