using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Services.StatusServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController(IStatusService _statusService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateStatus(CreateStatusDto dto)
        {
            await _statusService.CreateStatusAsync(dto);
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
