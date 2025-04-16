using Shared.Enums;
using SharedLibrary.Enums;

namespace Job.Business.Dtos.UserDtos;

public class UserPersonalInfoDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Position { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public string ImageUrl { get; set; }
    public DateTime BirthDay { get; set; }
    public Military MilitarySituation { get; set; }
    public Driver IsDriver { get; set; }
    public FamilySituation FamilySituation { get; set; }
}
