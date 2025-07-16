// backend/Services/Common/ServiceException.cs
namespace backend.Services.Common
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
        public ServiceException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class NotFoundException : ServiceException
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : ServiceException
    {
        public IEnumerable<string> Errors { get; }
        
        public ValidationException(string message) : base(message) 
        {
            Errors = new[] { message };
        }
        
        public ValidationException(IEnumerable<string> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : ServiceException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message) { }
    }
}
