using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.ExamExceptions;

public class ExamAlreadyTakenException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public ExamAlreadyTakenException()
    {
        ErrorMessage = "Siz bu imtahanda artıq iştirak etmisiniz.";
    }

    public ExamAlreadyTakenException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
