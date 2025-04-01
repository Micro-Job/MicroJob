using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.StatusExceptions;

public class CannotChangeStatusVisibilityException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public CannotChangeStatusVisibilityException()
    {
        ErrorMessage = "Statusa müraciət olduğu üçün statusu görünməz etmək olmaz.";
    }

    public CannotChangeStatusVisibilityException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
