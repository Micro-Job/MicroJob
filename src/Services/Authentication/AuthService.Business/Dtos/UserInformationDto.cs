namespace AuthService.Business.Dtos
{
    public class UserInformationDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
        public string? Image { get; set; }
        public UserStatusDto Status { get; set; }
    }
}