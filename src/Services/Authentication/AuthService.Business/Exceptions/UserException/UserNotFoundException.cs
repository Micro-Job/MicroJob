using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    internal class UserNotFoundException : Exception, IBaseException
    {
        public int StatusCode => throw new NotImplementedException();

        public string ErrorMessage { get; }

        public UserNotFoundException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("NOTFOUNDEXCEPTION_USER");
        }

        public UserNotFoundException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}