// backend/Services/Common/IResult.cs
namespace backend.Services.Common
{
    public interface IResult<T>
    {
        bool IsSuccess { get; }
        T? Data { get; }
        string? ErrorMessage { get; }
        IEnumerable<string>? Errors { get; }
    }

    public class Result<T> : IResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public IEnumerable<string>? Errors { get; private set; }

        private Result(bool isSuccess, T? data, string? errorMessage, IEnumerable<string>? errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            Errors = errors;
        }

        public static Result<T> Success(T data) => new(true, data, null, null);
        public static Result<T> Failure(string error) => new(false, default, error, null);
        public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, null, errors);
    }
}