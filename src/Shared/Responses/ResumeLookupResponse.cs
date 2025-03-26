using SharedLibrary.Enums;

namespace SharedLibrary.Responses;

public class ResumeLookupResponse
{
    public List<ResumeInfoDto> Resumes { get; set; } = [];
}

public class ResumeInfoDto
{
    public Guid UserId { get; set; }
    public Gender Gender { get; set; }
    public List<Guid> SkillIds { get; set; } = [];
}
