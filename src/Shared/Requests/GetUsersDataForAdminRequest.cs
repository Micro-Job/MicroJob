using SharedLibrary.Enums;

namespace SharedLibrary.Requests;

public class GetUsersDataForAdminRequest
{
    public UserRole UserRole { get; set; }
    public string? SearchTerm { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
