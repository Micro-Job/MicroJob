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
    public class WrongPasswordException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public WrongPasswordException()
        {
            ErrorMessage = MessageHelper.GetMessage("PASSWORDS_DO_NOT_MATCH");
        }
        public WrongPasswordException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
