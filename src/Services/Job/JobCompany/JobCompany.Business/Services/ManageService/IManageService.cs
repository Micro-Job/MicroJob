using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Services.ManageService;

public interface IManageService
{
    Task VacancyAcceptAsync(string vacancyId);
    Task VacancyRejectAsync(VacancyStatusUpdateDto dto);
    Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto);
    Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? vacancyName, string? startMinDate, string? startMaxDate, string? endMinDate, string? endMaxDate, string? companyName, byte? vacancyStatus, int skip = 1, int take = 10);


    Task<DataListDto<MessageWithTranslationsDto>> GetAllMessagesAsync(int pageNumber = 1, int pageSize = 10);
    Task<List<MessageSelectDto>> GetAllMessagesForSelectAsync();
    Task<MessageDto> GetMessageByIdAsync(string id);
    Task<Message> CreateMessageAsync(CreateMessageDto dto);
    Task UpdateMessageAsync(string id, UpdateMessageDto dto);
    Task DeleteMessageAsync(string id);

    Task<CompanyProfileDto> GetCompanyDetailsAsync(string companyUserId);
    Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyUserIdAsync(string companyUserId, int skip = 1, int take = 9);

    Task<DataListDto<CategoryWithTranslationsDto>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
    Task<CategoryDto> GetCategoryByIdAsync(string id);
    Task CreateCategoryAsync(CategoryCreateDto dto);
    Task UpdateCategoryAsync(string id, List<CategoryTranslationDto> categories);
    Task DeleteCategoryAsync(string id);
}
