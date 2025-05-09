using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.VacancyExceptions;

public class VacancyUpdateException : Exception,IBaseException
{
    public int StatusCode => StatusCodes.Status403Forbidden;

    public string ErrorMessage { get; }

    public VacancyUpdateException()
    {
        ErrorMessage = MessageHelper.GetMessage("VACANCY_UPDATE");
    }

    public VacancyUpdateException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
