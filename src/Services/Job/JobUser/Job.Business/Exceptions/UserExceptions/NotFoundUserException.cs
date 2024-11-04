using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Exceptions.Common;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.UserExceptions
{
    public class NotFoundUserException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public NotFoundUserException()
        {
            ErrorMessage = "User not found";
        }

        public NotFoundUserException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}