using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.ManageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole(UserRole.Admin , UserRole.Operator)]
    public class ManageController(IManageService _service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyAccept(string vacancyId)
        {
            await _service.VacancyAcceptAsync(vacancyId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyReject(VacancyStatusUpdateDto dto)
        {
            await _service.VacancyRejectAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleBlockVacancyStatus(VacancyStatusUpdateDto dto)
        {
            await _service.ToggleBlockVacancyStatusAsync(dto);
            return Ok();
        }
    }
}
