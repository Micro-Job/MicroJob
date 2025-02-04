using Shared.Dtos.VacancyDtos;

namespace Shared.Responses;

public class GetAllVacanciesResponse
{
    public ICollection<AllVacanyDto> Vacancies { get; set; }
    public int TotalCount { get; set; }
}