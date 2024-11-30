using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Services.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        readonly IApplicationService _service;

        public ApplicationController(IApplicationService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateApplication(ApplicationCreateDto dto)
        {
            await _service.CreateApplicationAsync(dto);
            return Ok();
        }

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

        [HttpPut("[action]")]
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