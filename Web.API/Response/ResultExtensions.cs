using Microsoft.AspNetCore.Mvc;
using Web.API.Common;
using Web.Domain.Primitives;

namespace Web.API.Response
{
    // Presentation/WebApi/Extensions/ResultExtensions.cs
    public static class ResultExtensions
    {
        // Cho Result (Command không trả về data)
        public static IActionResult ToActionResult(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(ApiResponse.Ok("Operation successful."));
            }

            return result.Error.Type switch
            {
                ErrorType.Validation => new BadRequestObjectResult(ApiResponse.Fail(result.Error.Code, result.Error.Message)),
                ErrorType.NotFound => new NotFoundObjectResult(ApiResponse.Fail(result.Error.Code, result.Error.Message)),
                ErrorType.Conflict => new ConflictObjectResult(ApiResponse.Fail(result.Error.Code, result.Error.Message)),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(ApiResponse.Fail(result.Error.Code, result.Error.Message)),
                _ => new ObjectResult(ApiResponse.Fail("InternalError", "An unexpected error occurred."))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }

        // Cho Result<T> (Query/Command trả về data)
        public static IActionResult ToActionResult<T>(this Result<T> result, HttpContext httpContext)
        {
            if (result.IsFailure)
            {
                // Tái sử dụng logic lỗi từ hàm trên
                return Result.Failure(result.Error).ToActionResult(httpContext);
            }

            // Nếu thành công, bọc Data vào ApiResponse
            return new OkObjectResult(ApiResponse<T>.Ok(result.Value));
        }
    }
}
