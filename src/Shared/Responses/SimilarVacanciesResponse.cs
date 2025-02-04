using SharedLibrary.Enums;

namespace Shared.Responses;

public class SimilarVacanciesResponse
{
    public List<SimilarVacancyResponse> Vacancies { get; set; } = [];
    public int TotalCount { get; set; } 
}

public class SimilarVacancyResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public string? CompanyLocation { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CompanyPhoto { get; set; }
    public decimal? MainSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public int? ViewCount { get; set; }
    public bool IsVip { get; set; }
    public bool IsSaved { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public WorkType? WorkType { get; set; }
}