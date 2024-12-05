using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.ApplicationExceptions
{
    public class ApplicationStatusIsDeactiveException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public ApplicationStatusIsDeactiveException()
        {
            ErrorMessage = "Müraciətin statusu deaktivdir.";
        }

        public ApplicationStatusIsDeactiveException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}