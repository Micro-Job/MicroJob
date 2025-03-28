using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.UserExceptions;

public class EmailAlreadyUsedException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public EmailAlreadyUsedException()
    {
        ErrorMessage = "Bu email artıq istifadə olunub";
    }

    public EmailAlreadyUsedException(string? message) : base(message)
    {
        ErrorMessage = message;
    }
}
