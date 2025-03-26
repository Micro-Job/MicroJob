using JobCompany.Business.Dtos.VacancyDtos;

namespace JobCompany.Business.Services.ManageService;

public interface IManageService
{
    Task VacancyAcceptAsync(VacancyAcceptDto dto);
    Task VacancyRejectAsync(VacancyStatusUpdateDto dto);
    Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto);
}
