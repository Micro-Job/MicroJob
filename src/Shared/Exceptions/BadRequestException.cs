using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace SharedLibrary.Exceptions
{
    public class BadRequestException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public BadRequestException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("BAD_REQUEST");
        }

        public BadRequestException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
