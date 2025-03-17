using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Services.StatusServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class StatusController(IStatusService _statusService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateStatus(CreateStatusDto dto)
        {
            await _statusService.CreateStatusAsync(dto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateStatus(List<UpdateStatusDto> dtos)
        {
            await _statusService.UpdateStatusAsync(dtos);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteStatus(string id)
        {
            await _statusService.DeleteStatusAsync(id);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllStatuses()
        {
            return Ok(await _statusService.GetAllStatusesAsync());
        }
    }
}