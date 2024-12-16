using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.Common;

public class EntityNotFoundException : Exception, IBaseException
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string ErrorMessage { get; }

    public EntityNotFoundException(string entityName)
    {
        ErrorMessage = $"{entityName} mövcud deyil";
    }

    public EntityNotFoundException(string entityName, string? message) : base(message)
    {
        ErrorMessage = message ?? $"{entityName} mövcud deyil";
    }
}

