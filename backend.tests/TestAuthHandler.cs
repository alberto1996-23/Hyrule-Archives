using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HyruleArchives.Tests;

// The test handler lets each test choose which logical user is making the request
// without depending on the real Google OAuth flow.
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string SubjectIdHeader = "X-Test-Subject-Id";
    public const string EmailHeader = "X-Test-Email";
    public const string DisplayNameHeader = "X-Test-Display-Name";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(SubjectIdHeader, out var subjectId) ||
            string.IsNullOrWhiteSpace(subjectId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, subjectId.ToString()),
            new(ClaimTypes.Email, Request.Headers[EmailHeader].FirstOrDefault() ?? $"{subjectId}@tests.local"),
            new(ClaimTypes.Name, Request.Headers[DisplayNameHeader].FirstOrDefault() ?? $"Test User {subjectId}")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
