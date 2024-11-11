using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;
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
            ErrorMessage = "Şifrələr uyğunlaşmır";
        }
        public WrongPasswordException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
