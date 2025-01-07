using Job.Business.Services.Application;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.SimpleUser)]
    public class ApplicationController(IUserApplicationService service) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplication(
            [FromQuery] int skip = 1,
            [FromQuery] int take = 9
        )
        {
            return Ok(await service.GetUserApplicationsAsync(skip, take));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplication(string vacancyId)
        {
            await service.CreateUserApplicationAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplicationByIdAsync(string applicationId)
        {
            return Ok(await service.GetUserApplicationByIdAsync(applicationId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamIntro(string vacancyId)
        {
            return Ok(await service.GetExamIntroAsync(vacancyId));
        }
    }
}
