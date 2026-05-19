namespace Web.Domain.Primitives
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            // Quy tắc: Nếu thành công thì Error phải là None. Nếu thất bại thì Error phải khác None.
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("Successful result cannot have an error.");
            }
            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("Failed result must have an error.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        // Factory method tạo thành công
        public static Result Success() => new Result(true, Error.None);

        // Factory method tạo thất bại
        public static Result Failure(Error error) => new Result(false, error);

        // Implicit operator: Cho phép viết tắt từ Error sang Result
        // Ví dụ: return Error.NotFound(...); compiler sẽ tự wrap thành Result.Failure(...)
        public static implicit operator Result(Error error) => Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        // Chỉ được lấy Value khi IsSuccess = true, nếu không sẽ ném lỗi logic
        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result.");

        // Factory method tạo thành công kèm dữ liệu
        public static Result<T> Success(T value) => new Result<T>(value, true, Error.None);

        // Factory method tạo thất bại (che dấu factory của base class)
        public new static Result<T> Failure(Error error) => new Result<T>(default!, false, error);

        // Implicit operator từ T sang Result<T>
        // Ví dụ: return user; (compiler tự wrap thành Result<User>.Success(user))
        public static implicit operator Result<T>(T value) => Success(value);

        // Implicit operator từ Error sang Result<T>
        // Ví dụ: return Error.NotFound(...); (compiler tự wrap thành Result<T>.Failure(...))
        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}
