using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class VacancyPausedException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public VacancyPausedException()
    {
        ErrorMessage = MessageHelper.GetMessage("VACANCY_PAUSED");
    }

    public VacancyPausedException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}

