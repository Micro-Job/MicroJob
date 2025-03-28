using Job.Business.Dtos.UserDtos;
using Job.Business.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserInformationService _service) : ControllerBase
    {
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserJobStatus(UserJobStatusUpdateDto dto)
        {
            await _service.UpdateUserJobStatusAsync(dto);
            return Ok();
        }
    }
}
