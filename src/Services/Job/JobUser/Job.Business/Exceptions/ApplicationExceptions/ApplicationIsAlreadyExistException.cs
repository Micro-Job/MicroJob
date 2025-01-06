using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace Job.Business.Exceptions.ApplicationExceptions
{
    public class ApplicationIsAlreadyExistException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public ApplicationIsAlreadyExistException()
        {
            ErrorMessage = "User not found";
        }

        public ApplicationIsAlreadyExistException(string? message)
            : base(message)
        {
            ErrorMessage = message;
        }
    }
}
