using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.ResumeExceptions;

public class ResumeIsPublicException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public ResumeIsPublicException()
    {
        ErrorMessage = "Resume is public";
    }

    public ResumeIsPublicException(string? message) : base(message)
    {
        ErrorMessage = message;
    }
}

