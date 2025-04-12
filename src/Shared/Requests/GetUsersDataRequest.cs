namespace Shared.Requests
{
    public class GetUsersDataRequest
    {
        public List<Guid> UserIds { get; set; }
        public string? FullName { get; set; }
    }
}