using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    public class RefreshTokenExpiredException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status419AuthenticationTimeout;

        public string ErrorMessage { get; }

        public RefreshTokenExpiredException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("LOGIN_REQUIRED");
        }

        public RefreshTokenExpiredException(string? message) : base(message)
        {
            ErrorMessage = message;
        }

    }
}
