using SharedLibrary.Exceptions.Common;

namespace SharedLibrary.Exceptions
{
    public class InvalidFormatException : Exception, IBaseException
    {
        public int StatusCode => throw new NotImplementedException();

        public string ErrorMessage => throw new NotImplementedException();
    }
}