using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Exceptions.ApplicationExceptions
{
    public class ApplicationStatusIsDeactiveException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public ApplicationStatusIsDeactiveException()
        {
            ErrorMessage = MessageHelper.GetMessage("APPLICATION_IS_DEACTIVE");
        }

        public ApplicationStatusIsDeactiveException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}