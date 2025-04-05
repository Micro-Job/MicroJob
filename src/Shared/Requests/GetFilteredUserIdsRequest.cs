using SharedLibrary.Enums;

namespace SharedLibrary.Requests;

public class GetFilteredUserIdsRequest
{
    public List<Guid> UserIds { get; set; }
    public Gender? Gender { get; set; }
    public List<Guid>? SkillIds { get; set; }
}
