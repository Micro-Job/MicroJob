using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.ApplicationDtos;

public record AllApplicationListDto
{
    public Guid UserId { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid ResumeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string VacancyName { get; set; }
    public Guid VacancyId { get; set; }
    public Guid? StatusId { get; set; }
    public StatusEnum Status { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime DateTime { get; set; }
    public Guid? ExamId { get; set; }
    public float? ExamPercent { get; set; }
}