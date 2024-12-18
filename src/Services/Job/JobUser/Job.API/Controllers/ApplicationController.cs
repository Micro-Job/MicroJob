using Job.Business.Services.Application;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        readonly IUserApplicationService _service;

        public ApplicationController(IUserApplicationService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserApplication([FromQuery] int skip = 1, [FromQuery] int take = 9)
        {
            return Ok(await _service.GetUserApplicationsAsync(skip, take));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplication(string vacancyId)
        {
            await _service.CreateUserApplicationAsync(vacancyId);
            return Ok();
        }
    }
}