using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class AccountLockedException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public DateTime LockDownEndDate { get; }

        public AccountLockedException(DateTime lockDownEndDate) : base()
        {
            LockDownEndDate = lockDownEndDate;
            var remainingTime = lockDownEndDate - DateTime.Now;

            ErrorMessage = $"Hesabınız keçici olaraq bağlandı. {lockDownEndDate} - tarixində təkrar yoxlayın.";
        }

        public AccountLockedException(DateTime lockDownEndDate, string? message) : base(message)
        {
            LockDownEndDate = lockDownEndDate;
            var remainingTime = lockDownEndDate - DateTime.Now;

            ErrorMessage = $"{message} {lockDownEndDate} - tarixində təkrar yoxlayın.";
        }
    }
}
