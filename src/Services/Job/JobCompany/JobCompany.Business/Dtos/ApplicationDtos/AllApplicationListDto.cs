namespace JobCompany.Business.Dtos.ApplicationDtos;

public record AllApplicationListDto
{
    public Guid ApplicationId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string VacancyName { get; set; }
    public Guid VacancyId { get; set; }
    public Guid? StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime DateTime { get; set; }
}