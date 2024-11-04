using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Exceptions.Common
{
    public class CreateException<T> : Exception, IBaseException where T : BaseEntity
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public CreateException()
        {
            ErrorMessage = typeof(T).Name + " created failed for some reason";
        }

        public CreateException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}