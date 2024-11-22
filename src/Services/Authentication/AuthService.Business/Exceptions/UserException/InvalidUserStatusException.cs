namespace AuthService.Business.Exceptions.UserException
{
    public class InvalidUserStatusException : Exception
    {
        public InvalidUserStatusException()
            : base("The provided user status is invalid.")
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