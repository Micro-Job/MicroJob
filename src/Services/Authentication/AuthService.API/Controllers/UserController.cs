using AuthService.Business.Dtos;
using AuthService.Business.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

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

        [AuthorizeRole(UserRole.Admin, UserRole.Operator)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers(UserRole userRole, string? searchTerm, int pageIndex = 1, int pageSize = 10)
        {
            var result = await userService.GetAllUsersAsync(userRole, searchTerm, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserInformationAsync(UserUpdateDto dto)
        {
            return Ok(await userService.UpdateUserInformationAsync(dto));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserProfileImageAsync([FromForm] UserProfileImageUpdateDto dto)
        {
            return Ok(await userService.UpdateUserProfileImageAsync(dto));
        }
    }
}