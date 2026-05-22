using HyruleArchives.DTOs;
using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me")
            .WithTags("Current User")
            .RequireAuthorization();

        group.MapGet("/", async (HttpContext httpContext, AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var dto = new CurrentUserDto(
                user.Id,
                user.Email,
                user.DisplayName,
                user.IsSimulated,
                user.CreatedAt
            );

            return Results.Ok(dto);
        })
        .WithName("GetCurrentUser")
        .WithSummary("Gets the currently signed-in user.");

        return group;
    }
}