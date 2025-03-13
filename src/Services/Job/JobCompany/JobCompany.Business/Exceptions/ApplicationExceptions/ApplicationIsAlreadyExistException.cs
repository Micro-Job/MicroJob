using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.ApplicationExceptions;

public class ApplicationIsAlreadyExistException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public ApplicationIsAlreadyExistException()
    {
        ErrorMessage = "Müraciət artıq mövcuddur.";
    }

    public ApplicationIsAlreadyExistException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
