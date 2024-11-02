using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class RefreshTokenExpiredException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status419AuthenticationTimeout;

        public string ErrorMessage { get; }

        public RefreshTokenExpiredException() : base()
        {
            ErrorMessage = "Yenidən giriş etməlisiniz.";
        }

        public RefreshTokenExpiredException(string? message) : base(message)
        {
            ErrorMessage = message;
        }

    }
}
