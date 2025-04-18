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
    class ExistEmailException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status409Conflict;

        public string ErrorMessage { get; }

        public ExistEmailException() : base()
        {
            ErrorMessage = MessageHelper.GetMessage("USEREXISTEXCEPTION_EMAIL");
        }

        public ExistEmailException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
