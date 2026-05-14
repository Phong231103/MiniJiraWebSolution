namespace Web.Application.Common.Results
{
    public record Result<T> : IResult
    {
        public bool IsSuccess { get; init; }
        public T? Value { get; init; }
        public Error? Error { get; init; }
        public bool IsFailure => !IsSuccess;

        public Result(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(Error error) => new(false, default, error);
    }
}
