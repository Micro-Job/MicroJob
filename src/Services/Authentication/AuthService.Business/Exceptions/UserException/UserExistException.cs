using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    public class UserExistException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status409Conflict;

        public string ErrorMessage { get; }

        public UserExistException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("USER_PHONE_OR_EMAIL_EXISTS");
        }

        public UserExistException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}