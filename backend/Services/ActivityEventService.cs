using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Hubs;
using HyruleArchives.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;
// I use this service to create activity rows and broadcast them to connected clients.

public class ActivityEventService
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<ActivityHub> _hubContext;

    public ActivityEventService(
        AppDbContext dbContext,
        IHubContext<ActivityHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    // This method creates an ActivityEvent and broadcasts it live.
    //
    // You will call this from:
    // - comment create/edit/delete endpoints
    // - reaction like/dislike endpoints
    // - collection/favorite endpoints
    public async Task<ActivityEventDto> CreateActivityEventAsync(
        Guid appUserId,
        Guid? gameId,
        string type,
        string message,
        bool isPublic)
    {
        var activityEvent = new ActivityEvent
        {
            Id = Guid.NewGuid(),
            AppUserId = appUserId,
            GameId = gameId,
            Type = type,
            Message = message,
            IsPublic = isPublic,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.ActivityEvents.Add(activityEvent);
        await _dbContext.SaveChangesAsync();

        // Load the related user/game data so the frontend can show readable text.
        var savedEvent = await _dbContext.ActivityEvents
            .Include(activity => activity.AppUser)
            .Include(activity => activity.Game)
            .FirstAsync(activity => activity.Id == activityEvent.Id);

        var dto = ToDto(savedEvent);

        // Only public activity should go to the public live community feed.
        //
        // Examples:
        // - liked a game
        // - disliked a game
        // - commented on a game
        //
        // Private actions can still be saved in the database but not broadcast publicly.
        if (dto.IsPublic)
        {
            await _hubContext.Clients.All.SendAsync("ActivityCreated", dto);
        }

        return dto;
    }

    // Gets public activity for the public trends/recent activity page.
    public async Task<List<ActivityEventDto>> GetRecentPublicActivityAsync(int limit = 25)
    {   
        // SQLite cannot ORDER BY DateTimeOffset directly.
        // So we first load the matching public activity rows from the database,
        // then we sort them in C# using LINQ to Objects.
        var activityEvents = await _dbContext.ActivityEvents
            .Include(activity => activity.AppUser)
            .Include(activity => activity.Game)
            .Where(activity => activity.IsPublic)
            .ToListAsync();

        return activityEvents
            .OrderByDescending(activity => activity.CreatedAt)
            .Take(limit)
            .Select(ToDto)
            .ToList();
    }

    public async Task<List<ActivityEventDto>> GetRecentPublicActivityForGameAsync(Guid gameId, int limit = 25)
    {
        var activityEvents = await _dbContext.ActivityEvents
            .Include(activity => activity.AppUser)
            .Include(activity => activity.Game)
            .Where(activity => activity.IsPublic && activity.GameId == gameId)
            .ToListAsync();

        return activityEvents
            .OrderByDescending(activity => activity.CreatedAt)
            .Take(limit)
            .Select(ToDto)
            .ToList();
    }

    // Gets activity for one signed-in user.
    //
    // This can power the logged-in dashboard's "recent activity" section.
    public async Task<List<ActivityEventDto>> GetUserActivityAsync(Guid appUserId, int limit = 25)
    {
        var activityEvents = await _dbContext.ActivityEvents
            .Include(activity => activity.AppUser)
            .Include(activity => activity.Game)
            .Where(activity => activity.AppUserId == appUserId)
            .ToListAsync();

        return activityEvents
            .OrderByDescending(activity => activity.CreatedAt)
            .Take(limit)
            .Select(ToDto)
            .ToList();
    }

    private static ActivityEventDto ToDto(ActivityEvent activityEvent)
    {
        return new ActivityEventDto(
            activityEvent.Id,
            activityEvent.AppUserId,
            activityEvent.AppUser?.DisplayName,
            activityEvent.GameId,
            activityEvent.Game?.Title,
            activityEvent.Type,
            activityEvent.Message,
            activityEvent.IsPublic,
            activityEvent.CreatedAt
        );
    }
}