using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUserApplicationAsync(string vacancyId)
        {
            await _service.CreateUserApplicationAsync(vacancyId);
            return Ok();
        }
    }
}