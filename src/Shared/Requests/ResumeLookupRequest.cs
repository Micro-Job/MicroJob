using SharedLibrary.Enums;

namespace SharedLibrary.Requests;

public class ResumeLookupRequest
{
    public List<Guid> UserIds { get; set; } = [];
}