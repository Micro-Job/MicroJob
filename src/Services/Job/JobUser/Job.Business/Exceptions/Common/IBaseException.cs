using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Exceptions.Common
{
    public interface IBaseException
    {
        public int StatusCode { get; }
        public string ErrorMessage { get; }
    }
}