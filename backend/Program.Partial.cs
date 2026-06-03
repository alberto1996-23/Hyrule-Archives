// Expose the generated Program type to the integration test project.
// ASP.NET Core creates the actual Program class from top-level statements,
// and tests need this partial declaration so WebApplicationFactory can find it.
public partial class Program;