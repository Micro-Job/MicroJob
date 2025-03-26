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
    [Authorize]
    public class StatusController(IStatusService _statusService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllStatuses()
        {
            return Ok(await _statusService.GetAllStatusesAsync());
        }

        [AuthorizeRole(UserRole.CompanyUser , UserRole.EmployeeUser)]
        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeStatusOrderAsync([FromBody]List<ChangeStatusOrderDto> dtos)
        {
            await _statusService.ChangeSatusOrderAsync(dtos);
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