using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.VacancyDtos;

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

}
