using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class DontHavePermissionException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public DontHavePermissionException()
    {
        ErrorMessage = "Vakansiya statusunu dəyişmə icazəniz yoxdur.";
    }

    public DontHavePermissionException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
