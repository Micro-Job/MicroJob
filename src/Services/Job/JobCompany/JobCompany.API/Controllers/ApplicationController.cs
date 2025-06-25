using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser, UserRole.SimpleUser)]
    public class ApplicationController(ApplicationService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveApplication(string applicationId)
        {
            await service.RemoveApplicationAsync(applicationId);
            return Ok();
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllApplication(int skip = 1, int take = 9)
        //{
        //    return Ok(await service.GetAllApplicationAsync(skip, take));
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplicationsList(Guid? vacancyId, Gender? gender, StatusEnum? status, [FromQuery] List<Guid>? skillIds, string? fullName, int skip = 1, int take = 10)
        {
            return Ok(await service.GetAllApplicationsListAsync(vacancyId, gender, status, skillIds, fullName, skip, take));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangeApplicationStatus(Guid applicationId, Guid statusId)
        {
            await service.ChangeApplicationStatusAsync(applicationId, statusId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationById(Guid applicationId)
        {
            var data = await service.GetApplicationByIdAsync(applicationId);
            return Ok(data);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplication(Guid vacancyId)
        {
            await service.CreateUserApplicationAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplications(string? vacancyName, int skip = 1, int take = 9)
        {
            return Ok(await service.GetUserApplicationsAsync(vacancyName, skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplicationById(string applicationId)
        {
            return Ok(await service.GetUserApplicationByIdAsync(applicationId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplicationWithStatus()
        {
            return Ok(await service.GetAllApplicationWithStatusAsync());
        }
    }
}
