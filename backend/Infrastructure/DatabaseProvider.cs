namespace HyruleArchives.Infrastructure;

// Centralizing provider names avoids scattering string literals through startup code.
public static class DatabaseProvider
{
    public const string Sqlite = "Sqlite";
    public const string PostgreSql = "PostgreSql";
}