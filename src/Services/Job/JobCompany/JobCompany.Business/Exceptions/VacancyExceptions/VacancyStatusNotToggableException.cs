using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class VacancyStatusNotToggableException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public VacancyStatusNotToggableException()
    {
        ErrorMessage = "Vakansiya statusu dəyişdirilə bilməz.";
    }

    public VacancyStatusNotToggableException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
