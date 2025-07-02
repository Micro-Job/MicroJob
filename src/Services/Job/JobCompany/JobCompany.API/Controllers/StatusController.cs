using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Services.StatusServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class StatusController(StatusService _statusService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllStatuses()
        {
            return Ok(await _statusService.GetAllStatusesAsync());
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeStatusOrderAsync([FromBody]List<ChangeStatusOrderDto> dtos)
        {
            await _statusService.ChangeStatusOrderAsync(dtos);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ToggleChangeStatusVisibility(string statusId)
        {
            await _statusService.ToggleChangeStatusVisibilityAsync(statusId);
            return Ok();
        }
    }
}