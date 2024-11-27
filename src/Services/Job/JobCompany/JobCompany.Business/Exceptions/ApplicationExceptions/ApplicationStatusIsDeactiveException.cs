using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Exceptions.ApplicationExceptions
{
    public class ApplicationStatusIsDeactiveException : Exception,IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage {get;}

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