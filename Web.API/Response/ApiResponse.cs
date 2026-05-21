namespace Web.API.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ErrorCode { get; init; }
        public string? Message { get; init; }

        // Factory methods cho thành công
        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new ApiResponse<T> { Success = true, Data = data, Message = message };

        // Factory methods cho thất bại
        public static ApiResponse<T> Fail(string errorCode, string message) =>
            new ApiResponse<T> { Success = false, ErrorCode = errorCode, Message = message };
    }

    // Overload cho các Command không trả về dữ liệu (chỉ báo thành công/thất bại)
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Ok(string? message = null) =>
            new ApiResponse { Success = true, Message = message };

        public new static ApiResponse Fail(string errorCode, string message) =>
            new ApiResponse { Success = false, ErrorCode = errorCode, Message = message };
    }
}
