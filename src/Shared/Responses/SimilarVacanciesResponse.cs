using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace Shared.Responses
{
    public class SimilarVacanciesResponse
    {
        public List<SimilarVacancyResponse> Vacancies { get; set; } = new List<SimilarVacancyResponse>();
    }

    public class SimilarVacancyResponse
    {
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
        public WorkType? WorkType { get; set; }
    }
}