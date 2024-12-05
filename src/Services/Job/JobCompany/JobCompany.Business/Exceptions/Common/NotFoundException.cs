using JobCompany.Core.Entites.Base;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.Common
{
    public class NotFoundException<T> : Exception, IBaseException where T : BaseEntity
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public NotFoundException()
        {
            ErrorMessage = typeof(T).Name + " not found";
        }

        public NotFoundException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}