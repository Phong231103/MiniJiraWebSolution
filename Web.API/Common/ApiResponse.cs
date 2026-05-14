namespace Web.API.Common
{
    public record ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; }
        public IDictionary<string, string[]>? Errors { get; init; }
        public int StatusCode { get; init; }

        public static ApiResponse<T> Success(T data, string message = "Success", int statusCode = 200)
            => new()
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };

        public static ApiResponse<T> Failure(string message, int statusCode = 400, IDictionary<string, string[]>? errors = null)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
    }

    // ApiResponse (non-generic)
    public record ApiResponse : ApiResponse<object?>;
}
