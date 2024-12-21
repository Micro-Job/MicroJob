using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.UserExceptions
{
    public class UserIsNotLoggedInException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public UserIsNotLoggedInException()
        {
            ErrorMessage = "User is not logged in";
        }

        public UserIsNotLoggedInException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}