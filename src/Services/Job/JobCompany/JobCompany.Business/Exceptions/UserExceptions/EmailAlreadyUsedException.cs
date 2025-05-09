using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.UserExceptions;

public class EmailAlreadyUsedException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public EmailAlreadyUsedException()
    {
        ErrorMessage = MessageHelper.GetMessage("EMAIL_ALREADY_USED");
    }

    public EmailAlreadyUsedException(string? message) : base(message)
    {
        ErrorMessage = message;
    }
}
