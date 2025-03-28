using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.StatusExceptions;

public class CannotChangePendingStatusVisibilityException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public CannotChangePendingStatusVisibilityException()
    {
        ErrorMessage = "Gözləmədə(pending) statusunu görünməz etmək olmaz.";
    }

    public CannotChangePendingStatusVisibilityException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
