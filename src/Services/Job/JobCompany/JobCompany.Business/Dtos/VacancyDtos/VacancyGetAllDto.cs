﻿using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyGetAllDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public bool IsSaved { get; set; }
    }
}
