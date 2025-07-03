using JobCompany.Business.Dtos.NumberDtos;

namespace JobCompany.Business.Dtos.CompanyDtos;

public class CompanyProfileDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? CompanyLogo { get; set; }
    public string? Information { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? EmployeeCount { get; set; }
    public string? Category { get; set; }
    public Guid? CategoryId { get; set; }
    public ICollection<CompanyNumberDto>? CompanyNumbers { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public Guid? CountryId { get; set; }
    public string? City { get; set; }
    public Guid? CityId { get; set; }
    public string? Location { get; set; }
    public string? WebLink { get; set; }
}
