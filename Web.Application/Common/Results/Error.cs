namespace Web.Application.Common.Results
{
    public record Error
    {
        public string Code { get; init; }
        public string Message { get; init; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        // Predefined errors
        public static Error NotFound => new("NOT_FOUND", "Resource not found");
        public static Error Validation => new("VALIDATION_ERROR", "Validation failed");
        public static Error Unauthorized => new("UNAUTHORIZED", "Unauthorized access");
        public static Error Forbidden => new("FORBIDDEN", "Forbidden access");
        public static Error Required => new("REQUIRED", "Required field missing");
    }
}
