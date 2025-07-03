using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Exceptions.UserException
{
    public class ExistPhoneNumberException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status409Conflict;

        public string ErrorMessage { get; }

        public ExistPhoneNumberException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("USEREXISTEXCEPTION_PHONE");
        }

        public ExistPhoneNumberException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
