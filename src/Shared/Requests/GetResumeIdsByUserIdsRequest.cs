namespace SharedLibrary.Requests;

public class GetResumeIdsByUserIdsRequest
{
    public List<Guid> UserIds { get; set; } = [];
}
