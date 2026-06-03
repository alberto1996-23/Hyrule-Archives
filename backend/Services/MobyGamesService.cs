using System.Text.Json;
using HyruleArchives.DTOs;
using System.Text.RegularExpressions;

namespace HyruleArchives.Services;

public class MobyGamesService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MobyGamesService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<MobyGameDto>> SearchZeldaGamesAsync()
    {
        // Read the API key from user-secrets or environment variables.
        // This keeps the key out of GitHub and out of the React frontend.
        var apiKey = _configuration["MobyGames:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("MobyGames API key is not configured.");
        }

        // MobyGames docs say query arguments, including the API key,
        // should be URL encoded. This is especially important if the key
        // contains symbols like +, /, or =.
        var encodedApiKey = Uri.EscapeDataString(apiKey);

        // Search for Zelda titles.
        //
        // title=zelda means "title contains zelda".
        // format=normal gives us richer data like descriptions, platforms,
        // sample cover, and sample screenshots.
        // limit=100 gives the frontend enough Zelda titles to build a more useful
        // chronological series list from the original 1986 release through modern entries.
        var url =
            $"https://api.mobygames.com/v1/games" +
            $"?api_key={encodedApiKey}" +
            $"&title=zelda" +
            $"&format=normal" +
            $"&limit=100";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"MobyGames API request failed with status code {(int)response.StatusCode}."
            );
        }

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var root = document.RootElement;

        var results = new List<MobyGameDto>();

        // MobyGames returns the list inside a "games" array.
        if (!root.TryGetProperty("games", out var gamesElement) ||
            gamesElement.ValueKind != JsonValueKind.Array)
        {
            return results;
        }

        foreach (var gameElement in gamesElement.EnumerateArray())
        {
            results.Add(ParseGame(gameElement));
        }

        return results;
    }

    // This method fetches one exact MobyGames title by id.
    // It powers the import endpoint so Swagger can turn a third-party record
    // into a local record that the rest of the app can interact with.
    public async Task<MobyGameDto?> GetGameByIdAsync(int mobyGameId)
    {
        var apiKey = _configuration["MobyGames:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("MobyGames API key is not configured.");
        }

        var encodedApiKey = Uri.EscapeDataString(apiKey);
        var url =
            $"https://api.mobygames.com/v1/games/{mobyGameId}" +
            $"?api_key={encodedApiKey}" +
            $"&format=normal";

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"MobyGames API request failed with status code {(int)response.StatusCode}."
            );
        }

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        return ParseGame(document.RootElement);
    }

    private static MobyGameDto ParseGame(JsonElement gameElement)
    {
        var gameId = GetInt(gameElement, "game_id");
        var title = GetString(gameElement, "title") ?? "Unknown Title";
        var description = StripHtml(GetString(gameElement, "description"));
        var mobyUrl = GetString(gameElement, "moby_url");
        var mobyScore = GetNullableDouble(gameElement, "moby_score");

        var coverImage = GetNestedString(gameElement, "sample_cover", "image");
        var coverThumbnail = GetNestedString(gameElement, "sample_cover", "thumbnail_image");

        string? screenshotImage = null;

        if (gameElement.TryGetProperty("sample_screenshots", out var screenshotsElement) &&
            screenshotsElement.ValueKind == JsonValueKind.Array)
        {
            var firstScreenshot = screenshotsElement.EnumerateArray().FirstOrDefault();

            if (firstScreenshot.ValueKind == JsonValueKind.Object)
            {
                screenshotImage = GetString(firstScreenshot, "image");
            }
        }

        var platforms = new List<MobyPlatformDto>();

        if (gameElement.TryGetProperty("platforms", out var platformsElement) &&
            platformsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var platformElement in platformsElement.EnumerateArray())
            {
                var platformId = GetInt(platformElement, "platform_id");
                var platformName = GetString(platformElement, "platform_name") ?? "Unknown Platform";
                var firstReleaseDate = GetString(platformElement, "first_release_date");

                platforms.Add(new MobyPlatformDto(
                    platformId,
                    platformName,
                    firstReleaseDate
                ));
            }
        }

        return new MobyGameDto(
            gameId,
            title,
            description,
            mobyScore,
            mobyUrl,
            coverImage,
            coverThumbnail,
            screenshotImage,
            platforms,
            "Data by MobyGames.com"
        );
    }

    // Helper method for safely reading string values from JSON.
    // This prevents crashes if MobyGames returns null or missing fields.
    private static string? GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return property.GetString();
    }

    // Helper method for safely reading int values from JSON.
    private static int GetInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return 0;
        }

        if (property.ValueKind != JsonValueKind.Number)
        {
            return 0;
        }

        return property.GetInt32();
    }

    // Helper method for safely reading optional decimal/double values.
    private static double? GetNullableDouble(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return property.GetDouble();
    }

    // Helper method for reading a nested string.
    // Example:
    // sample_cover.image
    private static string? GetNestedString(
        JsonElement element,
        string parentPropertyName,
        string childPropertyName)
    {
        if (!element.TryGetProperty(parentPropertyName, out var parent))
        {
            return null;
        }

        if (parent.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (parent.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return GetString(parent, childPropertyName);
    }

    private static string? StripHtml(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return Regex.Replace(value, "<.*?>", string.Empty).Trim();
    }
}