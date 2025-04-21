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
    [AuthorizeRole(UserRole.Admin, UserRole.Operator)]
    public class ManageController(IManageService _manageService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyAccept(string vacancyId)
        {
            await _manageService.VacancyAcceptAsync(vacancyId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyReject(VacancyStatusUpdateDto dto)
        {
            await _manageService.VacancyRejectAsync(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleBlockVacancyStatus(VacancyStatusUpdateDto dto)
        {
            await _manageService.ToggleBlockVacancyStatusAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacancies(string? vacancyName, string? startMinDate, string? startMaxDate, string? endMinDate, string? endMaxDate, string? companyName, byte? vacancyStatus, int skip = 1, int take = 10)
        {
            return Ok(await _manageService.GetAllVacanciesAsync(vacancyName, startMinDate, startMaxDate, endMinDate, endMaxDate, companyName, vacancyStatus, skip, take));
        }



        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessages(int pageNumber, int pageSize)
        {
            return Ok(await _manageService.GetAllMessagesAsync(pageNumber, pageSize));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            return Ok(await _manageService.GetMessageByIdAsync(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMessage(CreateMessageDto dto)
        {
            await _manageService.CreateMessageAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateMessage(string id, UpdateMessageDto dto)
        {
            await _manageService.UpdateMessageAsync(id, dto);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            await _manageService.DeleteMessageAsync(id);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetails(string companyUserId)
        {
            return Ok(await _manageService.GetCompanyDetailsAsync(companyUserId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacanciesByCompanyUserId(string companyUserId, int skip = 1, int take = 9)
        {
            return Ok(await _manageService.GetVacanciesByCompanyUserIdAsync(companyUserId, skip, take));
        }
    }
}
