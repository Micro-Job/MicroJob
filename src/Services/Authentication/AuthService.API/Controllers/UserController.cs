using AuthService.Business.Dtos;
using AuthService.Business.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserInformationAsync()
        {
            var data = await userService.GetUserInformationAsync();
            return Ok(data);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserInformationAsync(UserUpdateDto dto)
        {
            return Ok(await userService.UpdateUserInformationAsync(dto));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserProfileImageAsync([FromForm] UserProfileImageUpdateDto dto)
        {
            await userService.UpdateUserProfileImageAsync(dto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto)
        {
            await userService.UpdateUserJobStatusAsync(dto);
            return Ok();
        }
    }
}