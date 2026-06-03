using System.Net;
using System.Net.Http.Json;
using HyruleArchives.Models;
using Xunit;

namespace HyruleArchives.Tests;

public class EndpointAuthorizationTests
{
    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/me/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyActivity_WithoutAuthentication_ReturnsUnauthorized()
    {
        await using var factory = new BackendApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/me/activity/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOtherUsersComment_ReturnsForbidden()
    {
        await using var factory = new BackendApiFactory();
        var gameId = Guid.NewGuid();
        var owner = CreateAppUser("owner-user", "Owner User");
        var intruder = CreateAppUser("intruder-user", "Intruder User");
        var commentId = Guid.NewGuid();

        await factory.SeedAsync(async dbContext =>
        {
            dbContext.AppUsers.AddRange(owner, intruder);
            dbContext.Games.Add(CreateGame(gameId, "Forbidden Test Game"));
            dbContext.Comments.Add(new Comment
            {
                Id = commentId,
                AppUserId = owner.Id,
                GameId = gameId,
                Body = "Original owner comment",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            await Task.CompletedTask;
        });

        using var client = factory.CreateAuthenticatedClient("intruder-user", "Intruder User");
        var response = await client.PutAsJsonAsync($"/api/comments/{commentId}", new { body = "Trying to edit another user comment" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOtherUsersComment_ReturnsForbidden()
    {
        await using var factory = new BackendApiFactory();
        var gameId = Guid.NewGuid();
        var owner = CreateAppUser("owner-user", "Owner User");
        var intruder = CreateAppUser("intruder-user", "Intruder User");
        var commentId = Guid.NewGuid();

        await factory.SeedAsync(async dbContext =>
        {
            dbContext.AppUsers.AddRange(owner, intruder);
            dbContext.Games.Add(CreateGame(gameId, "Forbidden Delete Test Game"));
            dbContext.Comments.Add(new Comment
            {
                Id = commentId,
                AppUserId = owner.Id,
                GameId = gameId,
                Body = "Original owner comment",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            await Task.CompletedTask;
        });

        using var client = factory.CreateAuthenticatedClient("intruder-user", "Intruder User");
        var response = await client.DeleteAsync($"/api/comments/{commentId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyCollection_ReturnsOnlyTheCurrentUsersItems()
    {
        await using var factory = new BackendApiFactory();
        var owner = CreateAppUser("collector-a", "Collector A");
        var otherUser = CreateAppUser("collector-b", "Collector B");
        var ownerGameId = Guid.NewGuid();
        var otherGameId = Guid.NewGuid();

        await factory.SeedAsync(async dbContext =>
        {
            dbContext.AppUsers.AddRange(owner, otherUser);
            dbContext.Games.AddRange(
                CreateGame(ownerGameId, "Owner Game"),
                CreateGame(otherGameId, "Other Game"));
            dbContext.CollectionItems.AddRange(
                new CollectionItem
                {
                    Id = Guid.NewGuid(),
                    AppUserId = owner.Id,
                    GameId = ownerGameId,
                    Status = "Saved",
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                new CollectionItem
                {
                    Id = Guid.NewGuid(),
                    AppUserId = otherUser.Id,
                    GameId = otherGameId,
                    Status = "Completed",
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

            await Task.CompletedTask;
        });

        using var client = factory.CreateAuthenticatedClient("collector-a", "Collector A");
        var items = await client.GetFromJsonAsync<List<CollectionItemResponse>>("/api/me/collection/");

        Assert.NotNull(items);
        Assert.Single(items!);
        Assert.Equal(ownerGameId, items[0].GameId);
        Assert.Equal("Owner Game", items[0].GameTitle);
    }

    [Fact]
    public async Task GetMyFavorites_ReturnsOnlyTheCurrentUsersFavoriteItems()
    {
        await using var factory = new BackendApiFactory();
        var owner = CreateAppUser("favorite-a", "Favorite A");
        var otherUser = CreateAppUser("favorite-b", "Favorite B");
        var ownerGameId = Guid.NewGuid();
        var otherGameId = Guid.NewGuid();

        await factory.SeedAsync(async dbContext =>
        {
            dbContext.AppUsers.AddRange(owner, otherUser);
            dbContext.Games.AddRange(
                CreateGame(ownerGameId, "Owner Favorite"),
                CreateGame(otherGameId, "Other Favorite"));
            dbContext.CollectionItems.AddRange(
                new CollectionItem
                {
                    Id = Guid.NewGuid(),
                    AppUserId = owner.Id,
                    GameId = ownerGameId,
                    Status = "Saved",
                    IsFavorite = true,
                    FavoritedAt = DateTimeOffset.UtcNow,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                new CollectionItem
                {
                    Id = Guid.NewGuid(),
                    AppUserId = otherUser.Id,
                    GameId = otherGameId,
                    Status = "Saved",
                    IsFavorite = true,
                    FavoritedAt = DateTimeOffset.UtcNow,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

            await Task.CompletedTask;
        });

        using var client = factory.CreateAuthenticatedClient("favorite-a", "Favorite A");
        var items = await client.GetFromJsonAsync<List<CollectionItemResponse>>("/api/me/collection/favorites");

        Assert.NotNull(items);
        Assert.Single(items!);
        Assert.Equal(ownerGameId, items[0].GameId);
        Assert.Equal("Owner Favorite", items[0].GameTitle);
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

    private sealed record CollectionItemResponse(Guid Id, Guid GameId, string GameTitle, string Status);
}
