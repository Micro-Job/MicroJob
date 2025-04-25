using SharedLibrary.Enums;

namespace AuthService.Business.Dtos;

public class OperatorInfoDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public UserRole UserRole { get; set; }
}

public class OperatorAddDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public UserRole UserRole { get; set; }
}

public class OperatorUpdateDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
    public UserRole UserRole { get; set; }
}
