namespace HyruleArchives.Infrastructure;
// I use this constants file so startup code can compare provider names without magic strings everywhere.

// Centralizing provider names avoids scattering string literals through startup code.
public static class DatabaseProvider
{
    public const string Sqlite = "Sqlite";
    public const string PostgreSql = "PostgreSql";
}