namespace SharedLibrary.Responses;

public class GetResumeIdsByUserIdsResponse
{
    /// <summary> Bu dictionary-də Key tərəfi UserId, Value tərəfi isə ResumeId-dir. </summary>
    public Dictionary<Guid, Guid> ResumeIds { get; set; } = [];
}
