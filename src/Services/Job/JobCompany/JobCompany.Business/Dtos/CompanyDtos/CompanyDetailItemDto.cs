using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyDetailItemDto
    {
        public string UserId { get; set; }
        public string? CompanyInformation { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public ICollection<CompanyNumberDto>? CompanyNumbers { get; set; }
        public string? CompanyLocation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? WebLink { get; set; }
    }
}   