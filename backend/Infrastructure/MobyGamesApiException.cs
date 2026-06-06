namespace HyruleArchives.Infrastructure;

// I use this exception type when I need to pass the upstream MobyGames status code back up the stack.
public sealed class MobyGamesApiException : Exception
{
    public int StatusCode { get; }

    public MobyGamesApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}