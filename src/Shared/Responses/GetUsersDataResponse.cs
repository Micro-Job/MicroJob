namespace Shared.Responses
{
    public class GetUsersDataResponse
    {
        public List<UserResponse> Users { get; set; } = new List<UserResponse>();
    }
    public class UserResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImage { get; set; }
    }
}