using System.Security.Claims;
using HyruleArchives.Data;
using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;

// This service translates an authenticated ClaimsPrincipal into an AppUser row.
// If the OAuth user logs in for the first time, the service creates their local profile.
public class AuthUserService
{
    private readonly AppDbContext _dbContext;

    public AuthUserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppUser?> GetOrCreateCurrentUserAsync(ClaimsPrincipal principal)
    {
        // If ASP.NET Core did not authenticate the request, there is no local user to load.
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var subjectId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(subjectId))
        {
            return null;
        }

        // Right now the app is using Google login, so the provider string is fixed.
        var provider = "Google";

        var existingUser = await _dbContext.AppUsers
            .FirstOrDefaultAsync(user =>
                user.OAuthProvider == provider &&
                user.OAuthSubjectId == subjectId);

        if (existingUser is not null)
        {
            return existingUser;
        }

        // First-time login: create a local profile that other tables can reference safely.
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var displayName = principal.FindFirstValue(ClaimTypes.Name);

        var newUser = new AppUser
        {
            Id = Guid.NewGuid(),
            OAuthProvider = provider,
            OAuthSubjectId = subjectId,
            Email = email,
            DisplayName = displayName,
            IsSimulated = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.AppUsers.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return newUser;
    }
}