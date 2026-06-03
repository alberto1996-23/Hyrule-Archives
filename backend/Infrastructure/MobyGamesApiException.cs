namespace HyruleArchives.Infrastructure;

// This exception keeps the upstream MobyGames status code so endpoint handlers
// can return a more accurate response than a generic internal server error.
public sealed class MobyGamesApiException : Exception
{
    public int StatusCode { get; }

    public MobyGamesApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}