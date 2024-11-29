using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.VacancyExceptions
{
    public class VacancyStatusIsDeactiveException : Exception,IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage {get;}

        public VacancyStatusIsDeactiveException()
        {
            ErrorMessage = "Vakansiya statusu deaktivdir.";
        }

        public VacancyStatusIsDeactiveException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}