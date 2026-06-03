namespace HyruleArchives.Infrastructure;

// These helpers keep API error responses consistent across all endpoint modules.
// Every non-successful result now returns Problem Details instead of ad-hoc JSON.
public static class ApiErrors
{
    public static IResult BadRequest(string detail, string title = "Bad request") =>
        Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: title, detail: detail);

    public static IResult Unauthorized(string detail = "Authentication is required to access this resource.") =>
        Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized", detail: detail);

    public static IResult Forbidden(string detail = "You do not have access to this resource.") =>
        Results.Problem(statusCode: StatusCodes.Status403Forbidden, title: "Forbidden", detail: detail);

    public static IResult NotFound(string detail, string title = "Resource not found") =>
        Results.Problem(statusCode: StatusCodes.Status404NotFound, title: title, detail: detail);

    public static IResult Conflict(string detail, string title = "Conflict") =>
        Results.Problem(statusCode: StatusCodes.Status409Conflict, title: title, detail: detail);

    public static IResult InternalServerError(string detail, string title = "Server error") =>
        Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: title, detail: detail);
}
