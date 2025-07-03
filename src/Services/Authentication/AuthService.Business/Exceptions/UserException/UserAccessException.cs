using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    public class UserAccessException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public UserAccessException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("NO_PERMISSION");
        }

        public UserAccessException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}