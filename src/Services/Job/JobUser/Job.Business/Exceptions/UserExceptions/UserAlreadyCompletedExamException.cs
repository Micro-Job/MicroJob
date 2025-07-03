using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.UserExceptions;

public class UserAlreadyCompletedExamException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;
    public string ErrorMessage { get; }
    public UserAlreadyCompletedExamException()
    {
        ErrorMessage = "User already completed exam";
    }
    public UserAlreadyCompletedExamException(string message) : base(message)
    {
        ErrorMessage = message;
    }
}
