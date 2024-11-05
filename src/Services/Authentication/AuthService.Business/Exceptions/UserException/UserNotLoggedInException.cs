namespace AuthService.Business.Exceptions.UserException
{
    public class UserNotLoggedInException : Exception
    {
        public UserNotLoggedInException()
            : base("Istifadəçi giriş etməyib!")
        {
        }

        public UserNotLoggedInException(string message)
            : base(message)
        {
        }

        public UserNotLoggedInException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}