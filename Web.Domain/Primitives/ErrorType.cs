namespace Web.Domain.Primitives
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Required = 5,
        Exist = 6
    }
}
