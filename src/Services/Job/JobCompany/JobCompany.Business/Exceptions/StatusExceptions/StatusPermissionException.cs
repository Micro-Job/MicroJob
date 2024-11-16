using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Exceptions.StatusExceptions
{
    public class StatusPermissionException : Exception,IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage {get;}

        public StatusPermissionException()
        {
            ErrorMessage = "Statusu silməyə icazəniz yoxdur";
        }

        public StatusPermissionException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
