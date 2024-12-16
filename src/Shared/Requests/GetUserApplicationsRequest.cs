namespace SharedLibrary.Requests;

public class GetUserApplicationsRequest
{
    public Guid UserId { get; set; }
    public int Skip { get; set; } = 1;
    public int Take { get; set; } = 9;
}   