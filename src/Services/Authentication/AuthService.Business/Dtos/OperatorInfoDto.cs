using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace AuthService.Business.Dtos;

public class OperatorInfoDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public UserRole UserRole { get; set; }
}
