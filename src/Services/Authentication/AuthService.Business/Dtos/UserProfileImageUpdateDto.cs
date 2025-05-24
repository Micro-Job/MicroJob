using Microsoft.AspNetCore.Http;

namespace AuthService.Business.Dtos;

public class UserProfileImageUpdateDto
{
    public Guid UserId { get; set; }
    public IFormFile? Image { get; set; }
}