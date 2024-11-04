using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.Common
{
    public class IsAlreadyExistException<T> : Exception, IBaseException where T : BaseEntity
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public IsAlreadyExistException()
        {
            ErrorMessage = typeof(T).Name + " is already exists";
        }

        public IsAlreadyExistException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}