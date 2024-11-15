using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace Shared.Exceptions
{
    public class MustBeUniqueException<T> : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status404NotFound;

        public string ErrorMessage { get; }

        public MustBeUniqueException() : base()
        {
            ErrorMessage = $"{typeof(T).Name} artıq mövcuddur.";
        }

        public MustBeUniqueException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}