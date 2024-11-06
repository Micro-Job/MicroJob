using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;

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