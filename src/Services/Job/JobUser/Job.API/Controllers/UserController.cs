using Job.Business.Dtos.UserDtos;
using Job.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserInformationService _service) : ControllerBase
    {
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserJobStatus(UserJobStatusUpdateDto dto)
        {
            await _service.UpdateUserJobStatusAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserInformation()
        {
            return Ok(await _service.GetUserInformationAsync());
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserInformation(UpdateUserDto dto)
        {
            await _service.UpdateUserInformationAsync(dto);
            return Ok();
        }
    }
}
