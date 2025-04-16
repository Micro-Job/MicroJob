namespace SharedLibrary.Dtos.ResumeDtos;

public class ExperienceDto
{
    public string OrganizationName { get; set; }
    public string PositionName { get; set; }
    public string? PositionDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentOrganization { get; set; }
}

public class LanguageDto
{
    public byte LanguageName { get; set; }
    public byte LanguageLevel { get; set; }
}

public class EducationDto
{
    public string InstitutionName { get; set; }
    public string Profession { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentEducation { get; set; }
    public byte ProfessionDegree { get; set; }
}

public class CertificateDto
{
    public string CertificateName { get; set; }
    public string GivenOrganization { get; set; }
    public string CertificateFile { get; set; }
}