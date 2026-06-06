using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to group the current-user routes behind one consistent surface.

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
                return ApiErrors.Unauthorized();
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