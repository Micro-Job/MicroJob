using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser, UserRole.SimpleUser)]
    public class ApplicationController(IApplicationService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveApplication(string applicationId)
        {
            await service.RemoveApplicationAsync(applicationId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplication(int skip = 1, int take = 9)
        {
            return Ok(await service.GetAllApplicationAsync(skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplicationsList(int skip = 1, int take = 10)
        {
            return Ok(await service.GetAllApplicationsListAsync(skip, take));
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> ChangeApplicationStatus(string applicationId,string statusId)
        {
            await service.ChangeApplicationStatusAsync(applicationId, statusId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationById(string applicationId)
        {
            var data = await service.GetApplicationByIdAsync(applicationId);
            return Ok(data);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplication(string vacancyId)
        {
            await service.CreateUserApplicationAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplications(int skip = 1, int take = 9)
        {
            return Ok(await service.GetUserApplicationsAsync(skip, take));
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
