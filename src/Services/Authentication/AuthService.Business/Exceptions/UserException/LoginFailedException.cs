using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    public class LoginFailedException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status401Unauthorized;

        public string ErrorMessage { get; }

        public LoginFailedException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("AUTHENTICATION_FAILED");
        }

        public LoginFailedException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}