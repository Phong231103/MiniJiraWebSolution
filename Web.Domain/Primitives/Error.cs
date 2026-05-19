namespace Web.Domain.Primitives
{
    public record Error
    {
        // Sử dụng record vì nó immutable (bất biến) và tự so sánh giá trị (value equality)

        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string code, string message, ErrorType type)
        {
            Code = code;
            Message = message;
            Type = type;
        }

        // Các factory method tĩnh để tạo lỗi cho dễ đọc
        public static Error NotFound(string code, string message) =>
            new Error(code, message, ErrorType.NotFound);

        public static Error Validation(string code, string message) =>
            new Error(code, message, ErrorType.Validation);

        public static Error Conflict(string code, string message) =>
            new Error(code, message, ErrorType.Conflict);

        public static Error Unauthorized(string code, string message) =>
            new Error(code, message, ErrorType.Unauthorized);

        public static Error Required(string code, string message) =>
            new Error(code, message, ErrorType.Required);

        public static Error Exist(string code, string message) =>
            new Error(code, message, ErrorType.Exist);

        // Đại diện cho trạng thái không có lỗi (Rất quan trọng cho Result)
        public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.Failure);
    }
}
