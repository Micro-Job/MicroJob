using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class UserNotLoggedInException : Exception, IBaseException
    {
        public string ErrorMessage { get; }
        public int StatusCode => StatusCodes.Status401Unauthorized;

        public UserNotLoggedInException() : base()
        {
            ErrorMessage = "Istifadəçi giriş etməyib!";
        }

        public UserNotLoggedInException(string? message): base(message)
        {
            ErrorMessage = message;
        }
    }
}