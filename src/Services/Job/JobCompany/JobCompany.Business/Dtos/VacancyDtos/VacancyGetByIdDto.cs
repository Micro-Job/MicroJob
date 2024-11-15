﻿using JobCompany.Core.Entites;
using JobCompany.Core.Enums;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyGetByIdDto
    {
        public Guid Id { get; set; }
        public Company Company { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public Country Country { get; set; }
        public City City { get; set; }
        //public string? Email { get; set; }
        public ICollection<CompanyNumber>? CompanyNumbers { get; set; }
        public WorkType? WorkType { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        public bool IsActive { get; set; }
        public Category Category { get; set; }
    }
}