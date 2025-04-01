using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.ApplicationDtos;

public class ApplicationWithStatusInfoListDto
{
    public Guid ApplicationId { get; set; }
    public string? ProfileImage { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public Guid? StatusId { get; set; }
    public string? StatusName { get; set; }
}
