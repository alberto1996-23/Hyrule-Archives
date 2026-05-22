using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HyruleArchives.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapGet("/login", (IConfiguration configuration) =>
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var clientSecret = configuration["Authentication:Google:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return Results.Problem(
                    title: "Google OAuth is not configured.",
                    detail: "Set Authentication:Google:ClientId and Authentication:Google:ClientSecret using user-secrets or environment variables.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = "/swagger"
            };

            return Results.Challenge(properties, new[] { "Google" });
        })
        .WithName("Login")
        .WithSummary("Starts the Google OAuth login flow.");

        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Ok(new
            {
                message = "Logged out successfully."
            });
        })
        .RequireAuthorization()
        .WithName("Logout")
        .WithSummary("Logs out the current user.");

        return group;
    }
}