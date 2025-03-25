using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.ManageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageController(IManageService _service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyAccept(VacancyAcceptDto dto)
        {
            await _service.VacancyAcceptAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyReject(VacancyStatusUpdateDto dto)
        {
            await _service.VacancyRejectAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyBlock(VacancyStatusUpdateDto dto)
        {
            await _service.VacancyBlockAsync(dto);
            return Ok();
        }
    }
}
