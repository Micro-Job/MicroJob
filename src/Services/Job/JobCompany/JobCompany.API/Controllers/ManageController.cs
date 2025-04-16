using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.ManageService;
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessages()
        {
            return Ok(await _service.GetAllMessagesAsync());
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            return Ok(await _service.GetMessageByIdAsync(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMessage(CreateMessageDto dto)
        {
            await _service.CreateMessageAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateMessage(string id, UpdateMessageDto dto)
        {
            await _service.UpdateMessageAsync(id, dto);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            await _service.DeleteMessageAsync(id);
            return Ok();
        }
    }
}
