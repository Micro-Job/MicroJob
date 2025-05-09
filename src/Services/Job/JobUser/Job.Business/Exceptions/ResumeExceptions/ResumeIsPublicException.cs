using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace Job.Business.Exceptions.ResumeExceptions;

public class ResumeIsPublicException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public ResumeIsPublicException()
    {
        ErrorMessage = MessageHelper.GetMessage("RESUME_IS_PUBLIC");
    }

    public ResumeIsPublicException(string? message) : base(message)
    {
        ErrorMessage = message;
    }
}

