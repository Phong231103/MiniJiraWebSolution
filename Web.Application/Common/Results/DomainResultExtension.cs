namespace Web.Application.Common.Results
{
    public static class DomainResultExtension
    {
        public static Result<T> ToApplicationResult<T>(Result<T> domainResult)
            => domainResult.IsSuccess
                ? Result<T>.Success(domainResult.Value!)
                : Result<T>.Failure(domainResult.Error!);

    }
}
