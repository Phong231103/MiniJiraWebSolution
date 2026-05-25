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

        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
        public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
        public static Error Required(string code, string message) => new(code, message, ErrorType.Required);
        public static Error Exist(string code, string message) => new(code, message, ErrorType.Exist);
        public static Error Fail(string code, string message) => new(code, message, ErrorType.Failure);

        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    }
}
