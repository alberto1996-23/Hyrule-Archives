using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;
// I use this service to centralize local game import, lookup, and upsert logic.

// This service owns write operations for local Game records.
// Keeping import logic here keeps the endpoint focused on HTTP concerns.
public class GameService
{
	private readonly AppDbContext _dbContext;

	public GameService(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<MobyGameImportResultDto?> GetImportResultIfExistsAsync(int mobyGameId)
	{
		var externalApiId = $"mobygames-{mobyGameId}";

		var existingGame = await _dbContext.Games
			.Include(game => game.Releases)
			.FirstOrDefaultAsync(game => game.ExternalApiId == externalApiId);

		if (existingGame is null)
		{
			return null;
		}

		return new MobyGameImportResultDto(
			existingGame.Id,
			existingGame.ExternalApiId ?? externalApiId,
			existingGame.Title,
			existingGame.Slug,
			false,
			0,
			existingGame.Releases
				.Select(release => release.PlatformId)
				.Distinct()
				.Count());
	}

	public async Task<MobyGameImportResultDto> UpsertFromMobyGamesAsync(MobyGameDto mobyGame)
	{
		var externalApiId = $"mobygames-{mobyGame.MobyGameId}";
		var normalizedSlug = ToSlug(mobyGame.Title);

		// Reuse the same local row on repeated imports so reactions and comments stay attached.
		var existingGame = await _dbContext.Games
			.Include(game => game.Releases)
			.FirstOrDefaultAsync(game => game.ExternalApiId == externalApiId);

		// If an earlier local or seeded record already exists for the same Zelda title,
		// upgrade that row in place instead of creating a duplicate alongside it.
		if (existingGame is null)
		{
			existingGame = await _dbContext.Games
				.Include(game => game.Releases)
				.FirstOrDefaultAsync(game =>
					game.Title == mobyGame.Title ||
					game.Slug == normalizedSlug);
		}

		var wasCreated = existingGame is null;
		var game = existingGame ?? new Game
		{
			Id = Guid.NewGuid(),
			ExternalApiId = externalApiId,
			CreatedAt = DateTimeOffset.UtcNow
		};

		game.Title = mobyGame.Title;
		game.Summary = mobyGame.Description;
		game.ImageUrl = mobyGame.CoverImage ?? mobyGame.CoverThumbnail ?? mobyGame.ScreenshotImage;
		game.ExternalApiId = externalApiId;
		game.Slug = await GenerateUniqueSlugAsync(mobyGame.Title, game.Id);

		if (wasCreated)
		{
			_dbContext.Games.Add(game);
		}

		var globalRegion = await GetOrCreateGlobalRegionAsync();
		var releasesImported = 0;

		// MobyGames gives us platform names and first release dates.
		// We turn those into local Platform/GameRelease rows so the imported game
		// behaves like the rest of the local catalog.
		foreach (var mobyPlatform in mobyGame.Platforms)
		{
			var platform = await GetOrCreatePlatformAsync(mobyPlatform.PlatformName);

			if (!TryParseReleaseDate(mobyPlatform.FirstReleaseDate, out var releaseDate))
			{
				continue;
			}

			var releaseAlreadyExists = game.Releases.Any(release =>
				release.PlatformId == platform.Id &&
				release.RegionId == globalRegion.Id &&
				release.ReleaseDate.Date == releaseDate.Date);

			if (releaseAlreadyExists)
			{
				continue;
			}

			game.Releases.Add(new GameRelease
			{
				Id = Guid.NewGuid(),
				GameId = game.Id,
				PlatformId = platform.Id,
				RegionId = globalRegion.Id,
				ReleaseDate = releaseDate,
				Source = "MobyGames"
			});

			releasesImported++;
		}

		await _dbContext.SaveChangesAsync();

		return new MobyGameImportResultDto(
			game.Id,
			game.ExternalApiId ?? externalApiId,
			game.Title,
			game.Slug,
			wasCreated,
			releasesImported,
			mobyGame.Platforms.Count);
	}

	private async Task<Platform> GetOrCreatePlatformAsync(string platformName)
	{
		var existingPlatform = await _dbContext.Platforms
			.FirstOrDefaultAsync(platform => platform.Name == platformName);

		if (existingPlatform is not null)
		{
			return existingPlatform;
		}

		var platform = new Platform
		{
			Id = Guid.NewGuid(),
			Name = platformName,
			Slug = await GenerateUniquePlatformSlugAsync(platformName)
		};

		_dbContext.Platforms.Add(platform);
		await _dbContext.SaveChangesAsync();

		return platform;
	}

	private async Task<Region> GetOrCreateGlobalRegionAsync()
	{
		var existingRegion = await _dbContext.Regions
			.FirstOrDefaultAsync(region => region.Code == "GLOBAL");

		if (existingRegion is not null)
		{
			return existingRegion;
		}

		var region = new Region
		{
			Id = Guid.NewGuid(),
			Name = "Global",
			Code = "GLOBAL"
		};

		_dbContext.Regions.Add(region);
		await _dbContext.SaveChangesAsync();

		return region;
	}

	private async Task<string> GenerateUniqueSlugAsync(string title, Guid currentGameId)
	{
		var baseSlug = ToSlug(title);
		var candidate = baseSlug;
		var suffix = 2;

		while (await _dbContext.Games.AnyAsync(game => game.Slug == candidate && game.Id != currentGameId))
		{
			candidate = $"{baseSlug}-{suffix}";
			suffix++;
		}

		return candidate;
	}

	private async Task<string> GenerateUniquePlatformSlugAsync(string platformName)
	{
		var baseSlug = ToSlug(platformName);
		var candidate = baseSlug;
		var suffix = 2;

		while (await _dbContext.Platforms.AnyAsync(platform => platform.Slug == candidate))
		{
			candidate = $"{baseSlug}-{suffix}";
			suffix++;
		}

		return candidate;
	}

	private static bool TryParseReleaseDate(string? value, out DateTime releaseDate)
	{
		releaseDate = default;

		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		return DateTime.TryParse(value, out releaseDate);
	}

	private static string ToSlug(string value)
	{
		var slug = new string(value
			.Trim()
			.ToLowerInvariant()
			.Select(character => char.IsLetterOrDigit(character) ? character : '-')
			.ToArray());

		while (slug.Contains("--", StringComparison.Ordinal))
		{
			slug = slug.Replace("--", "-", StringComparison.Ordinal);
		}

		return slug.Trim('-');
	}
}
