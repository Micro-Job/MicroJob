﻿using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace AuthService.Business.Exceptions.UserException
{
    public class LoginFailedException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status401Unauthorized;

        public string ErrorMessage { get; }

        public LoginFailedException() : base()
        {
            ErrorMessage = "Giriş uğursuz oldu.";
        }

        public LoginFailedException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}