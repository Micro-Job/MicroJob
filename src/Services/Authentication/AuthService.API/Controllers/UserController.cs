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
    public class UserController(UserService userService) : ControllerBase
    {
        [AuthorizeRole(UserRole.Admin, UserRole.Operator)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers(UserRole userRole, string? fullName, string? email, string? phoneNumber, int pageIndex = 1, int pageSize = 10)
        {
            var result = await userService.GetAllUsersAsync(userRole, fullName, email, phoneNumber, pageIndex, pageSize);
            return Ok(result);
        }

        [AuthorizeRole(UserRole.Admin)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOperators(UserRole? userRole, string? fullName, string? email, string? phoneNumber, int pageIndex = 1, int pageSize = 10)
        {
            var result = await userService.GetAllOperatorsAsync(userRole, fullName, email, phoneNumber, pageIndex, pageSize);
            return Ok(result);
        }

        [AuthorizeRole(UserRole.Admin)]
        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetOperatorById(string userId)
        {
            var result = await userService.GetOperatorByIdAsync(userId);
            return Ok(result);
        }

        [AuthorizeRole(UserRole.Admin)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddOperator(OperatorAddDto dto)
        {
            await userService.AddOperatorAsync(dto);
            return Ok();
        }

        [AuthorizeRole(UserRole.Admin)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateOperator(OperatorUpdateDto dto)
        {
            await userService.UpdateOperatorAsync(dto);
            return Ok();
        }
    }
}