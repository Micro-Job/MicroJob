using AuthService.Business.Dtos;
using AuthService.Business.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserInformationAsync()
        {
            var data = await _userService.GetUserInformationAsync();
            return Ok(data);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserInformationAsync(UserUpdateDto dto)
        {
            await _userService.UpdateUserInformationAsync(dto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserProfileImageAsync([FromForm] UserProfileImageUpdateDto dto)
        {
            await _userService.UpdateUserProfileImageAsync(dto);
            return Ok();
        }
    }
}