using SharedLibrary.Helpers;

namespace AuthService.Business.Exceptions.UserException
{
    public class InvalidUserStatusException : Exception
    {
        public InvalidUserStatusException()
            : base(MessageHelper.GetMessage("INVALID_USER_STATUS"))
        {
        }

        public InvalidUserStatusException(string message)
            : base(message)
        {
        }

        public InvalidUserStatusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}