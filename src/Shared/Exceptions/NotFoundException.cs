using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace SharedLibrary.Exceptions
{
    public class NotFoundException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status404NotFound;

        public string ErrorMessage { get; }

        public NotFoundException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("NOT_FOUND");
        }

        public NotFoundException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}