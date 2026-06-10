using System.Net;
using System.Net.Http.Json;
using HyruleArchives.Models;
using Xunit;

namespace HyruleArchives.Tests;

// I keep these tests focused on the protected endpoint contract so I can verify auth and owner flows stay stable.
public class ProtectedEndpointBehaviorTests
{
    [Fact]
    public async Task AddCollectionItem_WithoutAuthentication_ReturnsUnauthorized()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.PostAsJsonAsync("/api/me/collection/", new { gameId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateComment_WithoutAuthentication_ReturnsUnauthorized()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.PostAsJsonAsync($"/api/games/{Guid.NewGuid()}/comments", new { body = "No auth" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SetReaction_WithoutAuthentication_ReturnsUnauthorized()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.PutAsJsonAsync($"/api/games/{Guid.NewGuid()}/reaction", new { value = "Like" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOwnComment_ReturnsOk()
    {
        await using var factory = new BackendApiFactory();
        var owner = CreateAppUser("comment-owner", "Comment Owner");
        var gameId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        await factory.SeedAsync(async dbContext =>
        {
            dbContext.AppUsers.Add(owner);
            dbContext.Games.Add(CreateGame(gameId, "Editable Test Game"));
            dbContext.Comments.Add(new Comment
            {
                Id = commentId,
                AppUserId = owner.Id,
                GameId = gameId,
                Body = "Original comment",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            await Task.CompletedTask;
        });

        using var client = factory.CreateAuthenticatedClient("comment-owner", "Comment Owner");
        var response = await client.PutAsJsonAsync($"/api/comments/{commentId}", new { body = "Updated comment body" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithAuthentication_ReturnsOk()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateAuthenticatedClient("current-user", "Current User");

        var response = await client.GetAsync("/api/me/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static AppUser CreateAppUser(string subjectId, string displayName)
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            OAuthProvider = "Google",
            OAuthSubjectId = subjectId,
            Email = $"{subjectId}@tests.local",
            DisplayName = displayName,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static Game CreateGame(Guid gameId, string title)
    {
        return new Game
        {
            Id = gameId,
            Title = title,
            Slug = title.ToLowerInvariant().Replace(' ', '-'),
            Summary = $"Summary for {title}",
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
