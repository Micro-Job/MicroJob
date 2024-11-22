﻿using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Exceptions.UserException
{
    public class PolicyException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage {get;}

        public PolicyException() : base()
        {
            ErrorMessage = "Şərtlərlə razı olmadan qeydiyyatdan keçə bilməzsiniz";
        }

        public PolicyException(string? message) : base(message)
        {
            ErrorMessage = message;
        }

    }
}
