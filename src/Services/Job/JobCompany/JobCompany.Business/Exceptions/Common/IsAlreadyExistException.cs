using JobCompany.Core.Entites.Base;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.Common
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