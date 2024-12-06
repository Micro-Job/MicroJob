using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace JobCompany.Business.Exceptions.ExamExceptions
{
    public class InvalidQuestionException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public InvalidQuestionException()
        {
            ErrorMessage = "Şəkil əsaslı suallar üçün şəkil URL-si tələb olunur.";
        }

        public InvalidQuestionException(string message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}