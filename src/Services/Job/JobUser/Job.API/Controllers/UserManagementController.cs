using Job.Business.Services.UserManagement;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizeRole(UserRole.Admin, UserRole.Operator)]
public class UserManagementController(UserManagementService _service) : ControllerBase
{
    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> GetPersonalInfo(Guid userId)
    {
        var result = await _service.GetPersonalInfoAsync(userId);
        return Ok(result);
    }

    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> GetResumeDetail(Guid userId)
    {
        var result = await _service.GetResumeDetailAsync(userId);
        return Ok(result);
    }
}