using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobPayment.Business.Exceptions.PaymentExceptions
{
    public class InsufficientBalanceException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status402PaymentRequired;

        public string ErrorMessage { get; }

        public InsufficientBalanceException()
        {
            ErrorMessage = "Yetərli balans yoxdur";
        }

        public InsufficientBalanceException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
