using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException;

public class OldPasswordWrongException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public OldPasswordWrongException() : base()
    {
        ErrorMessage = MessageHelper.GetMessage("OLD_PASSWORD_WRONG");
    }

    public OldPasswordWrongException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}