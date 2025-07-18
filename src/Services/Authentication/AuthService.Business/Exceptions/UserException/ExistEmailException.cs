using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;
using SharedLibrary.Helpers;
using System.Text.Json;

namespace AuthService.Business.Exceptions.UserException
{
    class ExistEmailException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status409Conflict;

        public string ErrorMessage { get; }

        public ExistEmailException() : base()
        {
            var error = new
            {
                errors = new
                {
                    email = MessageHelper.GetMessage("USEREXISTEXCEPTION_EMAIL")
                }
            };

            ErrorMessage = JsonSerializer.Serialize(error);
        }

        public ExistEmailException(string message) : base(message)
        {
            var error = new
            {
                errors = new
                {
                    email = message
                }
            };

            ErrorMessage = JsonSerializer.Serialize(error);
        }
    }
}
