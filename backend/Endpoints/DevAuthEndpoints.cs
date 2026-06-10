using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HyruleArchives.Endpoints;

// I keep this endpoint in a development-only module so automated tests can sign in without touching the real Google OAuth flow.
public static class DevAuthEndpoints
{
    public static RouteGroupBuilder MapDevAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/dev")
            .WithTags("Development Authentication");

        group.MapGet("/login", async (
            HttpContext httpContext,
            IConfiguration configuration,
            string? subjectId,
            string? displayName,
            string? email,
            string? returnTo) =>
        {
            var safeSubjectId = string.IsNullOrWhiteSpace(subjectId) ? "dev-user" : subjectId.Trim();
            var safeDisplayName = string.IsNullOrWhiteSpace(displayName) ? "Development User" : displayName.Trim();
            var safeEmail = string.IsNullOrWhiteSpace(email) ? $"{safeSubjectId}@local.dev" : email.Trim();
            var frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            var redirectTarget = string.IsNullOrWhiteSpace(returnTo) ? $"{frontendBaseUrl}/dashboard" : returnTo;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, safeSubjectId),
                new(ClaimTypes.Name, safeDisplayName),
                new(ClaimTypes.Email, safeEmail)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Redirect(redirectTarget);
        })
        .WithName("DevelopmentLogin")
        .WithSummary("Signs in a development-only cookie user for automated tests and local verification.");

        return group;
    }
}
