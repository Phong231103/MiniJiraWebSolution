//namespace Web.Domain.Common
//{
//    public record Result<T>
//    {
//        public bool IsSuccess { get; init; }
//        public T? Value { get; init; }
//        public Error? Error { get; init; }
//        public bool IsFailure => !IsSuccess;

//        protected Result(bool isSuccess, T? value, Error? error)
//        {
//            IsSuccess = isSuccess;
//            Value = value;
//            Error = error;
//        }

//        public static Result<T> Success(T value) => new(true, value, null);
//        public static Result<T> Failure(Error error) => new(false, default, error);

//        // Implicit conversion
//        public static implicit operator Result<T>(T value) => Success(value);
//        public static implicit operator T(Result<T> result) => result.Value!;
//    }
//}
