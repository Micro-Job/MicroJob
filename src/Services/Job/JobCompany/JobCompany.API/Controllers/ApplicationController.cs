using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController(IApplicationService service) : ControllerBase
    {
        readonly IApplicationService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveApplication(string applicationId)
        {
            await _service.RemoveApplicationAsync(applicationId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplication(int skip = 1, int take = 9)
        {
            return Ok(await _service.GetAllApplicationAsync(skip,take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllApplicationsList(int skip = 1, int take = 10)
        {
            return Ok(await _service.GetAllApplicationsListAsync(skip, take));
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> ChangeApplicationStatus(string applicationId, string statusId)
        {
            await _service.ChangeApplicationStatusAsync(applicationId, statusId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationById(string applicationId)
        {
            var data = await _service.GetApplicationByIdAsync(applicationId);
            return Ok(data);
        }
    }
}