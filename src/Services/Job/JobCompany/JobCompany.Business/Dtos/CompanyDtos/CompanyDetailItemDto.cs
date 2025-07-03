using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyDetailItemDto
    {
        public Guid UserId { get; set; }
        public string? CompanyInformation { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public ICollection<CompanyNumberDto>? CompanyNumbers { get; set; }
        public string? CompanyLocation { get; set; }
        public string Email { get; set; }
        public string? WebLink { get; set; }
        public int VacancyCount { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int EmployeeCount { get; set; }
    }
}   