﻿namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationUserListDto
    {
        public Guid VacancyId { get; set; }
        public string? VacancyImage { get; set; }
        public string VacancyTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public int? ViewCount { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
    }
}