using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.StatusExceptions;

public class CannotChangeStatusVisibilityException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public CannotChangeStatusVisibilityException()
    {
        ErrorMessage = MessageHelper.GetMessage("CANNOT_CHANGE_STATUS");
    }

    public CannotChangeStatusVisibilityException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
