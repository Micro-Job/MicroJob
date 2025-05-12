using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.StatusExceptions;

public class CannotChangePendingStatusVisibilityException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public CannotChangePendingStatusVisibilityException()
    {
        ErrorMessage = MessageHelper.GetMessage("CANNOT_CHANGE_PENDING_STATUS");
    }

    public CannotChangePendingStatusVisibilityException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
