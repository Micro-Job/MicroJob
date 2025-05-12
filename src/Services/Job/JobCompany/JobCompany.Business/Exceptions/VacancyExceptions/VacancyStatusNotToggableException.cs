using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class VacancyStatusNotToggableException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public VacancyStatusNotToggableException()
    {
        ErrorMessage = MessageHelper.GetMessage("VACANCY_STATUS_NOT_TOGGABLE");
    }

    public VacancyStatusNotToggableException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
