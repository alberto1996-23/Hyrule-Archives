using HyruleArchives.Data;
using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;
// I use this service to create or refresh demo data that makes the app easier to test locally.

public class SeedService
{
    private static readonly string[] CollectionStatuses = ["Saved", "Playing", "Completed", "Dropped"];
    private static readonly string[] ReactionValues = ["Like", "Dislike"];
    private static readonly string[] FirstNames =
    [
        "Link", "Zelda", "Impa", "Mipha", "Daruk", "Revali", "Urbosa", "Saria", "Ruto", "Malon",
        "Midna", "Rauru", "Purah", "Robbie", "Tulin", "Sidon", "Hestu", "Nabooru", "Aryll", "Marin"
    ];
    private static readonly string[] LastNames =
    [
        "of Hyrule", "the Hero", "the Scholar", "the Cartographer", "the Archivist", "the Collector",
        "of Kakariko", "of Akkala", "of Hebra", "of Gerudo", "of Lanayru", "of Eldin"
    ];
    private static readonly string[] CommentTemplates =
    [
        "I always come back to {0} because the world-building is excellent.",
        "The atmosphere in {0} still feels special every time I revisit it.",
        "{0} deserves more community discussion on the archive.",
        "This game has one of my favorite Zelda memories.",
        "I would absolutely recommend {0} to someone new to the series."
    ];
    private static readonly string[] ActivityTypes =
    [
        "CollectionAdded", "CollectionUpdated", "FavoriteAdded", "CommentCreated", "ReactionLiked", "ReactionDisliked"
    ];

    private readonly AppDbContext _dbContext;

    public SeedService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<object> SeedMilestone3Async()
    {
        var plan = new SeedPlan(
            Name: "Milestone 3",
            GamePrefix: "seed-m3-game-",
            UserPrefix: "sim-m3-user-",
            DomainRecordCount: 500,
            SimulatedUserCount: 50,
            CollectionItemCount: 1000,
            CommentCount: 0,
            ReactionCount: 0,
            ActivityEventCount: 0);

        return SeedScenarioAsync(plan, randomSeed: 436);
    }

    public Task<object> SeedFinalProjectAsync()
    {
        var plan = new SeedPlan(
            Name: "Final project",
            GamePrefix: "seed-final-game-",
            UserPrefix: "sim-final-user-",
            DomainRecordCount: 5000,
            SimulatedUserCount: 500,
            CollectionItemCount: 5000,
            CommentCount: 3000,
            ReactionCount: 2500,
            ActivityEventCount: 4000);

        return SeedScenarioAsync(plan, randomSeed: 3600);
    }

    private async Task<object> SeedScenarioAsync(SeedPlan plan, int randomSeed)
    {
        var random = new Random(randomSeed);
        var now = DateTimeOffset.UtcNow;

        // Reset only simulated content so real OAuth users and curated seed rows remain untouched.
        await RemoveExistingSimulatedDataAsync();

        var simulatedGames = CreateSimulatedGames(plan, now);
        var simulatedUsers = CreateSimulatedUsers(plan, random, now);

        _dbContext.Games.AddRange(simulatedGames);
        _dbContext.AppUsers.AddRange(simulatedUsers);
        await _dbContext.SaveChangesAsync();

        // Seed each interaction layer separately to keep the generation logic understandable.
        var collectionItems = CreateCollectionItems(plan, simulatedUsers, simulatedGames, random, now);
        _dbContext.CollectionItems.AddRange(collectionItems);
        await _dbContext.SaveChangesAsync();

        var comments = CreateComments(plan, simulatedUsers, simulatedGames, random, now);
        _dbContext.Comments.AddRange(comments);
        await _dbContext.SaveChangesAsync();

        var reactions = CreateReactions(plan, simulatedUsers, simulatedGames, random, now);
        _dbContext.Reactions.AddRange(reactions);
        await _dbContext.SaveChangesAsync();

        var activityEvents = CreateActivityEvents(plan, simulatedUsers, simulatedGames, random, now);
        _dbContext.ActivityEvents.AddRange(activityEvents);
        await _dbContext.SaveChangesAsync();

        return new
        {
            message = $"{plan.Name} seed completed.",
            domainRecordsCreated = simulatedGames.Count,
            simulatedUsersCreated = simulatedUsers.Count,
            collectionItemsCreated = collectionItems.Count,
            commentsCreated = comments.Count,
            reactionsCreated = reactions.Count,
            activityEventsCreated = activityEvents.Count,
            totalInteractionRecordsCreated = collectionItems.Count + comments.Count + reactions.Count + activityEvents.Count
        };
    }

    private static List<Game> CreateSimulatedGames(SeedPlan plan, DateTimeOffset now)
    {
        var games = new List<Game>(plan.DomainRecordCount);

        for (var index = 1; index <= plan.DomainRecordCount; index++)
        {
            games.Add(new Game
            {
                Id = Guid.NewGuid(),
                ExternalApiId = $"{plan.GamePrefix}{index:00000}",
                Title = $"Simulated Zelda Archive Record {index:00000}",
                Slug = $"simulated-zelda-archive-record-{index:00000}",
                Summary =
                    $"Simulated archive record {index:00000} for The Hyrule Archives. " +
                    "These records make the community views feel populated before real users arrive.",
                ImageUrl = null,
                CreatedAt = now.AddDays(-(index % 365))
            });
        }

        return games;
    }

    private static List<AppUser> CreateSimulatedUsers(SeedPlan plan, Random random, DateTimeOffset now)
    {
        var users = new List<AppUser>(plan.SimulatedUserCount);

        for (var index = 1; index <= plan.SimulatedUserCount; index++)
        {
            var displayName = $"{FirstNames[index % FirstNames.Length]} {LastNames[index % LastNames.Length]} #{index:000}";

            users.Add(new AppUser
            {
                Id = Guid.NewGuid(),
                OAuthProvider = "Simulated",
                OAuthSubjectId = $"{plan.UserPrefix}{index:00000}",
                Email = $"simulated.user{index:00000}@hyrulearchives.local",
                DisplayName = displayName,
                IsSimulated = true,
                CreatedAt = now.AddDays(-random.Next(30, 900))
            });
        }

        return users;
    }

    private static List<CollectionItem> CreateCollectionItems(
        SeedPlan plan,
        IReadOnlyList<AppUser> users,
        IReadOnlyList<Game> games,
        Random random,
        DateTimeOffset now)
    {
        var items = new List<CollectionItem>(plan.CollectionItemCount);
        var usedPairs = new HashSet<string>(StringComparer.Ordinal);

        while (items.Count < plan.CollectionItemCount)
        {
            var user = users[random.Next(users.Count)];
            var game = games[random.Next(games.Count)];
            var pairKey = $"{user.Id}:{game.Id}";

            if (!usedPairs.Add(pairKey))
            {
                continue;
            }

            var createdAt = now.AddDays(-random.Next(1, 540));
            var isFavorite = random.NextDouble() < 0.35;

            items.Add(new CollectionItem
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Status = CollectionStatuses[random.Next(CollectionStatuses.Length)],
                IsFavorite = isFavorite,
                FavoritedAt = isFavorite ? createdAt.AddDays(random.Next(0, 21)) : null,
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddDays(random.Next(0, 21))
            });
        }

        return items;
    }

    private static List<Comment> CreateComments(
        SeedPlan plan,
        IReadOnlyList<AppUser> users,
        IReadOnlyList<Game> games,
        Random random,
        DateTimeOffset now)
    {
        var comments = new List<Comment>(plan.CommentCount);

        for (var index = 0; index < plan.CommentCount; index++)
        {
            var user = users[random.Next(users.Count)];
            var game = games[random.Next(games.Count)];
            var template = CommentTemplates[random.Next(CommentTemplates.Length)];
            var createdAt = now.AddDays(-random.Next(0, 365));

            comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Body = string.Format(template, game.Title),
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddMinutes(random.Next(0, 90)),
                IsDeleted = false,
                DeletedAt = null
            });
        }

        return comments;
    }

    private static List<Reaction> CreateReactions(
        SeedPlan plan,
        IReadOnlyList<AppUser> users,
        IReadOnlyList<Game> games,
        Random random,
        DateTimeOffset now)
    {
        var reactions = new List<Reaction>(plan.ReactionCount);
        var usedPairs = new HashSet<string>(StringComparer.Ordinal);

        while (reactions.Count < plan.ReactionCount)
        {
            var user = users[random.Next(users.Count)];
            var game = games[random.Next(games.Count)];
            var pairKey = $"{user.Id}:{game.Id}";

            if (!usedPairs.Add(pairKey))
            {
                continue;
            }

            var createdAt = now.AddDays(-random.Next(0, 365));

            reactions.Add(new Reaction
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Value = ReactionValues[random.Next(ReactionValues.Length)],
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddMinutes(random.Next(0, 180))
            });
        }

        return reactions;
    }

    private static List<ActivityEvent> CreateActivityEvents(
        SeedPlan plan,
        IReadOnlyList<AppUser> users,
        IReadOnlyList<Game> games,
        Random random,
        DateTimeOffset now)
    {
        var events = new List<ActivityEvent>(plan.ActivityEventCount);

        for (var index = 0; index < plan.ActivityEventCount; index++)
        {
            var user = users[random.Next(users.Count)];
            var game = games[random.Next(games.Count)];
            var type = ActivityTypes[random.Next(ActivityTypes.Length)];
            var createdAt = now.AddDays(-random.Next(0, 180));

            events.Add(new ActivityEvent
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Type = type,
                Message = BuildActivityMessage(type, user.DisplayName ?? "A user", game.Title),
                IsPublic = type is "FavoriteAdded" or "CommentCreated" or "ReactionLiked" or "ReactionDisliked",
                CreatedAt = createdAt
            });
        }

        return events;
    }

    private async Task RemoveExistingSimulatedDataAsync()
    {
        // Simulated rows are identified by either the Simulated OAuth provider or seed-* external ids.
        var simulatedUserIds = await _dbContext.AppUsers
            .Where(user => user.IsSimulated && user.OAuthProvider == "Simulated")
            .Select(user => user.Id)
            .ToListAsync();

        var simulatedGameIds = await _dbContext.Games
            .Where(game => game.ExternalApiId != null && game.ExternalApiId.StartsWith("seed-"))
            .Select(game => game.Id)
            .ToListAsync();

        var activityEvents = await _dbContext.ActivityEvents
            .Where(activity => simulatedUserIds.Contains(activity.AppUserId) ||
                               (activity.GameId.HasValue && simulatedGameIds.Contains(activity.GameId.Value)))
            .ToListAsync();

        var comments = await _dbContext.Comments
            .Where(comment => simulatedUserIds.Contains(comment.AppUserId) || simulatedGameIds.Contains(comment.GameId))
            .ToListAsync();

        var reactions = await _dbContext.Reactions
            .Where(reaction => simulatedUserIds.Contains(reaction.AppUserId) || simulatedGameIds.Contains(reaction.GameId))
            .ToListAsync();

        var collectionItems = await _dbContext.CollectionItems
            .Where(item => simulatedUserIds.Contains(item.AppUserId) || simulatedGameIds.Contains(item.GameId))
            .ToListAsync();

        var users = await _dbContext.AppUsers
            .Where(user => simulatedUserIds.Contains(user.Id))
            .ToListAsync();

        var games = await _dbContext.Games
            .Where(game => simulatedGameIds.Contains(game.Id))
            .ToListAsync();

        _dbContext.ActivityEvents.RemoveRange(activityEvents);
        _dbContext.Comments.RemoveRange(comments);
        _dbContext.Reactions.RemoveRange(reactions);
        _dbContext.CollectionItems.RemoveRange(collectionItems);
        _dbContext.AppUsers.RemoveRange(users);
        _dbContext.Games.RemoveRange(games);

        await _dbContext.SaveChangesAsync();
    }

    private static string BuildActivityMessage(string type, string displayName, string gameTitle)
    {
        return type switch
        {
            "CollectionAdded" => $"{displayName} added {gameTitle} to their collection.",
            "CollectionUpdated" => $"{displayName} updated their progress for {gameTitle}.",
            "FavoriteAdded" => $"{displayName} favorited {gameTitle}.",
            "CommentCreated" => $"{displayName} commented on {gameTitle}.",
            "ReactionLiked" => $"{displayName} liked {gameTitle}.",
            "ReactionDisliked" => $"{displayName} disliked {gameTitle}.",
            _ => $"{displayName} interacted with {gameTitle}."
        };
    }

    private sealed record SeedPlan(
        string Name,
        string GamePrefix,
        string UserPrefix,
        int DomainRecordCount,
        int SimulatedUserCount,
        int CollectionItemCount,
        int CommentCount,
        int ReactionCount,
        int ActivityEventCount);
}