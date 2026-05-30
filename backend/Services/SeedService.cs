using HyruleArchives.Data;
using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;

public class SeedService
{
    private readonly AppDbContext _dbContext;

    public SeedService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object> SeedMilestone3Async()
    {
        // Fixed random seed makes the generated data repeatable.
        // That means each run creates a similar pattern, which helps testing.
        var random = new Random(436);

        // Remove old simulated Milestone 3 data first.
        // This prevents duplicate seed records if you run the seed endpoint multiple times.
        await RemoveOldMilestone3SeedDataAsync();

        var createdAt = DateTimeOffset.UtcNow;

        var simulatedGames = new List<Game>();

        // 500 domain records.
        //
        // In your app, Game is one of the main domain entities.
        // These are simulated archive records used to make the project database large enough
        // for the course milestone.
        for (var i = 1; i <= 500; i++)
        {
            simulatedGames.Add(new Game
            {
                Id = Guid.NewGuid(),
                ExternalApiId = $"seed-m3-game-{i:000}",
                Title = $"Simulated Zelda Archive Record {i:000}",
                Slug = $"simulated-zelda-archive-record-{i:000}",
                Summary =
                    $"Simulated domain record {i:000} for The Hyrule Archives. " +
                    "This record helps the local database feel populated for Milestone 3.",
                ImageUrl = null,
                CreatedAt = createdAt
            });
        }

        _dbContext.Games.AddRange(simulatedGames);

        var simulatedUsers = new List<AppUser>();

        // 50 simulated user profiles.
        //
        // These users are not real OAuth users.
        // They exist so the project has multi-user activity before real users join.
        for (var i = 1; i <= 50; i++)
        {
            simulatedUsers.Add(new AppUser
            {
                Id = Guid.NewGuid(),
                OAuthProvider = "Simulated",
                OAuthSubjectId = $"sim-user-{i:000}",
                Email = $"simulated.user{i:000}@hyrulearchives.local",
                DisplayName = $"Hyrule Fan {i:000}",
                IsSimulated = true,
                CreatedAt = createdAt.AddDays(-random.Next(1, 365))
            });
        }

        _dbContext.AppUsers.AddRange(simulatedUsers);

        // Save games and users before creating interactions,
        // because CollectionItems reference both AppUsers and Games.
        await _dbContext.SaveChangesAsync();

        var collectionItems = new List<CollectionItem>();
        var statuses = new[] { "Saved", "Playing", "Completed", "Dropped" };

        // This set prevents duplicate user/game pairs.
        // Your database has a unique index on AppUserId + GameId,
        // so duplicate pairs would cause an error.
        var usedPairs = new HashSet<string>();

        // 1,000 interaction records.
        //
        // A CollectionItem counts as an interaction because it represents
        // a user saving or tracking a game in their collection.
        while (collectionItems.Count < 1000)
        {
            var user = simulatedUsers[random.Next(simulatedUsers.Count)];
            var game = simulatedGames[random.Next(simulatedGames.Count)];

            var pairKey = $"{user.Id}:{game.Id}";

            if (usedPairs.Contains(pairKey))
            {
                continue;
            }

            usedPairs.Add(pairKey);

            collectionItems.Add(new CollectionItem
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Status = statuses[random.Next(statuses.Length)],
                CreatedAt = createdAt.AddDays(-random.Next(0, 180))
            });
        }

        _dbContext.CollectionItems.AddRange(collectionItems);

        await _dbContext.SaveChangesAsync();

        return new
        {
            message = "Milestone 3 seed completed.",
            domainRecordsCreated = simulatedGames.Count,
            simulatedUsersCreated = simulatedUsers.Count,
            interactionRecordsCreated = collectionItems.Count
        };
    }

    private async Task RemoveOldMilestone3SeedDataAsync()
    {
        // Find old simulated users from previous seed runs.
        var oldSimulatedUserIds = await _dbContext.AppUsers
            .Where(user => user.IsSimulated && user.OAuthProvider == "Simulated")
            .Select(user => user.Id)
            .ToListAsync();

        // Find old simulated games from previous seed runs.
        var oldSimulatedGameIds = await _dbContext.Games
            .Where(game => game.ExternalApiId != null &&
                           game.ExternalApiId.StartsWith("seed-m3-game-"))
            .Select(game => game.Id)
            .ToListAsync();

        // Remove old interaction records first because they reference users/games.
        var oldCollectionItems = await _dbContext.CollectionItems
            .Where(item =>
                oldSimulatedUserIds.Contains(item.AppUserId) ||
                oldSimulatedGameIds.Contains(item.GameId))
            .ToListAsync();

        _dbContext.CollectionItems.RemoveRange(oldCollectionItems);

        var oldUsers = await _dbContext.AppUsers
            .Where(user => oldSimulatedUserIds.Contains(user.Id))
            .ToListAsync();

        _dbContext.AppUsers.RemoveRange(oldUsers);

        var oldGames = await _dbContext.Games
            .Where(game => oldSimulatedGameIds.Contains(game.Id))
            .ToListAsync();

        _dbContext.Games.RemoveRange(oldGames);

        await _dbContext.SaveChangesAsync();
    }
}