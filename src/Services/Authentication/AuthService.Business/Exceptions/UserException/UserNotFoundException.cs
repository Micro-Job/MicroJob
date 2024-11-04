using SharedLibrary.Exceptions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Exceptions.UserException
{
    internal class UserNotFoundException : Exception, IBaseException
    {
        public int StatusCode => throw new NotImplementedException();

        public string ErrorMessage { get; }

        public UserNotFoundException() : base()
        {
            ErrorMessage = "İstifadəçi mövcud deyil";
        }

        public UserNotFoundException(string? message) :base(message)
        {
            ErrorMessage = message;   
        }
    }
}
