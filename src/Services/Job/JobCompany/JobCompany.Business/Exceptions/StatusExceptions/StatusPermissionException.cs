﻿using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.StatusExceptions
{
    public class StatusPermissionException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status403Forbidden;

        public string ErrorMessage { get; }

        public StatusPermissionException()
        {
            ErrorMessage = "Statusu dəyişməyə icazəniz yoxdur";
        }

        public StatusPermissionException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}