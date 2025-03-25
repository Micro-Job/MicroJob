using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class VacancyUpdateException : Exception,IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public VacancyUpdateException()
    {
        ErrorMessage = "Vakansiya blok olundugu ucun update edilmir.";
    }

    public VacancyUpdateException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
