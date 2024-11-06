using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class UserAccessException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public UserAccessException() : base()
        {
            ErrorMessage = "Bu əməliyyat üçün icazəniz yoxdur.";
        }

        public UserAccessException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}