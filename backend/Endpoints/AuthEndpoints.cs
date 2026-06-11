using HyruleArchives.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to group the login, logout, and auth callback routes.

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapGet("/login", (HttpContext httpContext, IConfiguration configuration) =>
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var clientSecret = configuration["Authentication:Google:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return ApiErrors.InternalServerError(
                    "Set Authentication:Google:ClientId and Authentication:Google:ClientSecret using user-secrets or environment variables.",
                    "Google OAuth is not configured.");
            }

            // Preserve the origin that actually started the OAuth flow so each browser returns
            // to the same localhost scheme/port it used at login time.
            var frontendBaseUrl = ResolveFrontendBaseUrl(httpContext, configuration);

            var properties = new AuthenticationProperties
            {
                RedirectUri = $"{frontendBaseUrl}/auth/callback"
            };

            // Always show Google's account picker so separate browser sessions can intentionally
            // choose different identities during demo flows and multi-user testing.
            properties.Parameters["prompt"] = "select_account";

            return Results.Challenge(properties, new[] { "Google" });
        })
        .WithName("Login")
        .WithSummary("Starts the Google OAuth login flow.");

        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            // Logout should be safe to call even if the browser already lost its auth cookie.
            // Making this endpoint idempotent prevents the frontend from hitting a redirect-based
            // auth failure when a logged-out user or expired session triggers logout again.
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Ok(new
            {
                message = "Logged out successfully."
            });
        })
        .WithName("Logout")
        .WithSummary("Logs out the current user.");

        return group;
    }

    private static string ResolveFrontendBaseUrl(HttpContext httpContext, IConfiguration configuration)
    {
        var configuredBaseUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
        // The frontend sends its current origin explicitly because Safari and Chrome may not
        // agree on which cached localhost scheme should be reused after the OAuth redirect.
        var requestedOrigin = httpContext.Request.Query["frontendOrigin"].ToString();

        if (string.IsNullOrWhiteSpace(requestedOrigin))
        {
            requestedOrigin = TryGetOriginFromReferer(httpContext.Request.Headers.Referer.ToString());
        }

        if (string.IsNullOrWhiteSpace(requestedOrigin) ||
            !Uri.TryCreate(requestedOrigin, UriKind.Absolute, out var requestedOriginUri))
        {
            return configuredBaseUrl;
        }

        var allowedOrigins = (configuration
                .GetSection("Frontend:AllowedOrigins")
                .Get<string[]>() ?? [])
            .Append(configuredBaseUrl)
            .OfType<string>()
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(NormalizeOrigin)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var normalizedRequestedOrigin = NormalizeOrigin(requestedOriginUri.GetLeftPart(UriPartial.Authority));

        // Never trust an arbitrary callback origin. Only return to an origin that is already
        // allowed by configuration/CORS; otherwise fall back to the configured base URL.
        return allowedOrigins.Contains(normalizedRequestedOrigin)
            ? normalizedRequestedOrigin
            : configuredBaseUrl;
    }

    private static string TryGetOriginFromReferer(string referer)
    {
        if (!Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
        {
            return string.Empty;
        }

        return refererUri.GetLeftPart(UriPartial.Authority);
    }

    private static string NormalizeOrigin(string origin)
    {
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
        {
            return origin;
        }

        var builder = new UriBuilder(uri)
        {
            Path = string.Empty,
            Query = string.Empty,
            Fragment = string.Empty
        };

        var isDefaultPort = (builder.Scheme == Uri.UriSchemeHttp && builder.Port == 80) ||
            (builder.Scheme == Uri.UriSchemeHttps && builder.Port == 443);

        if (isDefaultPort)
        {
            builder.Port = -1;
        }

        return builder.Uri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
    }
}