namespace SharedLibrary.Responses;

public class GetUsersDataForAdminResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
}
