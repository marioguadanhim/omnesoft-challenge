using ErrorOr;

namespace OmnesoftChallenge.Api.Endpoints.Errors;

public static class HandledErrorResponse
{
    public static object Body(string code, IEnumerable<string> descriptions)
    {
        List<string> errorsDescription = ["This is a Handled Error!", .. descriptions];

        return new { code, errorDescription = string.Join(" | ", errorsDescription) };
    }

    public static IResult ToHandledErrorResult(this List<Error> errors)
    {
        var firstError = errors.First();

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Json(Body(firstError.Code, errors.Select(error => error.Description)), statusCode: statusCode);
    }
}
