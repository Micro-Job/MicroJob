using Shared.Enums;
using SharedLibrary.Dtos.ResumeDtos;
using SharedLibrary.Enums;

namespace SharedLibrary.Responses;

public class GetResumeDetailResponse
{
    public Guid UserId { get; set; }
    public Guid ResumeId { get; set; }
    public bool IsSaved { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FatherName { get; set; }
    public string? Position { get; set; }
    public string? UserPhoto { get; set; }
    public Driver IsDriver { get; set; }
    public FamilySituation IsMarried { get; set; }
    public Citizenship IsCitizen { get; set; }
    public Gender Gender { get; set; }
    public Military MilitarySituation { get; set; }
    public string? Adress { get; set; }
    public DateTime BirthDay { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? ParentPositionId { get; set; }
    public bool IsMainEmail { get; set; }
    public bool IsMainNumber { get; set; }

    public string? ResumeEmail { get; set; }
    public ICollection<string>? Skills { get; set; }
    public ICollection<string>? PhoneNumbers { get; set; }
    public ICollection<EducationDto>? Educations { get; set; }
    public ICollection<ExperienceDto>? Experiences { get; set; }
    public ICollection<LanguageDto>? Languages { get; set; }
    public ICollection<CertificateDto>? Certificates { get; set; }
}
