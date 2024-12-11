using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace Shared.Dtos.VacancyDtos
{
    public class SimilarVacancyDto
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public decimal? MainSalary { get; set; }
        public WorkType? WorkType { get; set; }
        public bool IsSaved { get; set; }
        public bool IsVip { get; set; }
        public bool IsActive { get; set; }
        public Guid? CategoryId { get; set; }
    }
}