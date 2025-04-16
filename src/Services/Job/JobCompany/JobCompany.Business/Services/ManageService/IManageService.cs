using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.UserDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using SharedLibrary.Enums;
using SharedLibrary.Responses;

namespace JobCompany.Business.Services.ManageService;

public interface IManageService
{
    Task VacancyAcceptAsync(string vacancyId);
    Task VacancyRejectAsync(VacancyStatusUpdateDto dto);
    Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto);

    Task<List<MessageWithTranslationsDto>> GetAllMessagesAsync();
    Task<MessageDto> GetMessageByIdAsync(string id);
    Task CreateMessageAsync(CreateMessageDto dto);
    Task UpdateMessageAsync(string id, UpdateMessageDto dto);
    Task DeleteMessageAsync(string id);

    Task<DataListDto<GetUsersDataForAdminResponse>> GetAllUsersAsync(UserRole userRole, string? searchTerm, int pageIndex = 1, int pageSize = 10);
    Task<UserDetailsDto?> GetUserDetailsAsync(int tab, string userId);
}
