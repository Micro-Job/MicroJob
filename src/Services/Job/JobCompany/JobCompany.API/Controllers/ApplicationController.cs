using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController(ApplicationService service) : ControllerBase
    {
        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveApplication(Guid applicationId)
        {
            await service.RemoveApplicationAsync(applicationId);
            return Ok();
        }

        [AuthorizeRole(UserRole.EmployeeUser, UserRole.CompanyUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplicationsList([FromQuery] List<Guid>? vacancyIds, Gender? gender, [FromQuery] List<StatusEnum>? status, string? fullName, StatusEnum? skipStatus, int skip = 1, int take = 10)
        {
            return Ok(await service.GetAllApplicationsListAsync(vacancyIds, gender, status, fullName, skipStatus, skip, take));
        }

        [AuthorizeRole(UserRole.EmployeeUser, UserRole.CompanyUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplications([FromQuery] List<Guid>? vacancyIds, Gender? gender, [FromQuery] List<StatusEnum>? status, string? fullName, float? minPercent, float? maxPercent, int skip = 1, int take = 10)
        {
            return Ok(await service.GetAllApplicationsListAsync(vacancyIds, gender, status, fullName, minPercent, maxPercent, skip, take));
        }

        [AuthorizeRole(UserRole.EmployeeUser, UserRole.CompanyUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangeApplicationStatus(Guid applicationId, Guid statusId)
        {
            await service.ChangeApplicationStatusAsync(applicationId, statusId);
            return Ok();
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplication(Guid vacancyId)
        {
            return Ok(await service.CreateUserApplicationAsync(vacancyId));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplications(string? vacancyName, int skip = 1, int take = 9)
        {
            return Ok(await service.GetUserApplicationsAsync(vacancyName, skip, take));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplicationById(Guid applicationId)
        {
            return Ok(await service.GetUserApplicationByIdAsync(applicationId));
        }
    }
}
