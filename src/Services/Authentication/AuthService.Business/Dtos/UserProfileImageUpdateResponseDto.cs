namespace AuthService.Business.Dtos
{
    public class UserProfileImageUpdateResponseDto
    {
        public Guid UserId { get; set; }
        public string ImageUrl { get; set; }
    }
}