using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace SharedLibrary.Exceptions
{
    public class NotFoundException<T> : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status404NotFound;

        public string ErrorMessage { get; }

        public NotFoundException() : base()
        {
            ErrorMessage = $"{typeof(T).Name} mövcud deyil.";
        }

        public NotFoundException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}