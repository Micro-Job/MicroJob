using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.ApplicationExceptions;

public class ApplicationIsAlreadyExistException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public ApplicationIsAlreadyExistException()
    {
        ErrorMessage = MessageHelper.GetMessage("APPLICATION_ALREADY_EXIST");
    }

    public ApplicationIsAlreadyExistException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
