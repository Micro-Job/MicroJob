using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;
using System.Text.Json;

namespace AuthService.Business.Exceptions.UserException;

public class ExistUserException : Exception, SharedLibrary.Exceptions.Common.IBaseException
{
    public int StatusCode => StatusCodes.Status409Conflict;

    public string ErrorMessage { get; }

    public ExistUserException(bool emailExists, bool phoneExists) : base()
    {
        var errors = new Dictionary<string, string>();

        if (emailExists)
            errors.Add("email", MessageHelper.GetMessage("USEREXISTEXCEPTION_EMAIL"));

        if (phoneExists)
            errors.Add("phoneNumber", MessageHelper.GetMessage("USEREXISTEXCEPTION_PHONE"));

        var error = new
        {
            errors = errors
        };

        ErrorMessage = JsonSerializer.Serialize(error);
    }
}