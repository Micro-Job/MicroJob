using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class UserExistException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status409Conflict;

        public string ErrorMessage { get; }

        public UserExistException() : base()
        {
            ErrorMessage = "İstifadəçi nömrəsi və ya email mövcuddur.";
        }

        public UserExistException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}