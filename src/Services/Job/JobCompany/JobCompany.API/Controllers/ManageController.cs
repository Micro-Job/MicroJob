using JobCompany.Business.Dtos.CategoryDtos;
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
    [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.Operator)]
    public class ManageController(ManageService _manageService) : ControllerBase
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
        public async Task<IActionResult> GetByIdVacancy(Guid id)
        {
            return Ok(await _manageService.GetByIdVacancyAsync(id));
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessages(string? content, int pageNumber, int pageSize)
        {
            return Ok(await _manageService.GetAllMessagesAsync(content, pageNumber, pageSize));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessagesForSelect()
        {
            return Ok(await _manageService.GetAllMessagesForSelectAsync());
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetMessageById(Guid id)
        {
            return Ok(await _manageService.GetMessageByIdAsync(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMessage(CreateMessageDto dto)
        {
            return Ok(await _manageService.CreateMessageAsync(dto));
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateMessage(Guid id, UpdateMessageDto dto)
        {
            await _manageService.UpdateMessageAsync(id, dto);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            await _manageService.DeleteMessageAsync(id);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyDetails(Guid companyUserId)
        {
            return Ok(await _manageService.GetCompanyDetailsAsync(companyUserId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacanciesByCompanyUserId(Guid companyUserId, int skip = 1, int take = 9)
        {
            return Ok(await _manageService.GetVacanciesByCompanyUserIdAsync(companyUserId, skip, take));
        }



        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCategories(string? content , int pageNumber, int pageSize)
        {
            return Ok(await _manageService.GetAllCategoriesAsync(content , pageNumber, pageSize));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            return Ok(await _manageService.GetCategoryByIdAsync(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
        {
            await _manageService.CreateCategoryAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, List<CategoryTranslationDto> categories)
        {
            await _manageService.UpdateCategoryAsync(id, categories);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _manageService.DeleteCategoryAsync(id);
            return Ok();
        }
    }
}
