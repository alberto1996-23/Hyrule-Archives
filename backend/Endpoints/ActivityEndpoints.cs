using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;

public static class ActivityEndpoints
{
    public static RouteGroupBuilder MapActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/activity")
            .WithTags("Activity");

        // GET /api/activity/recent
        //
        // Public endpoint:
        // This powers the public trends/community activity feed.
        // It only returns activity events where IsPublic == true.
        group.MapGet("/recent", async (ActivityEventService activityEventService) =>
        {
            var activity = await activityEventService.GetRecentPublicActivityAsync();

            return Results.Ok(activity);
        })
        .WithName("GetRecentPublicActivity")
        .WithSummary("Gets recent public community activity.");

        return group;
    }

    public static RouteGroupBuilder MapMyActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me/activity")
            .WithTags("My Activity")
            .RequireAuthorization();

        // GET /api/me/activity
        //
        // Protected endpoint:
        // This powers the signed-in user's dashboard activity feed.
        group.MapGet("/", async (
            HttpContext httpContext,
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var activity = await activityEventService.GetUserActivityAsync(user.Id);

            return Results.Ok(activity);
        })
        .WithName("GetMyActivity")
        .WithSummary("Gets the signed-in user's recent activity.");

        return group;
    }
}