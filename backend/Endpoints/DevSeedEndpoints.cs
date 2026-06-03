using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;

public static class DevSeedEndpoints
{
    public static RouteGroupBuilder MapDevSeedEndpoints(this IEndpointRouteBuilder app)
    {
        // This endpoint is for local development only.
        // It should not be exposed as a normal production feature.
        var group = app.MapGroup("/api/dev/seed")
            .WithTags("Development Seed");

        // POST /api/dev/seed/milestone3
        //
        // This creates:
        // - 500 simulated domain records
        // - 50 simulated users
        // - 1,000 simulated collection interactions
        group.MapPost("/milestone3", async (SeedService seedService) =>
        {
            var result = await seedService.SeedMilestone3Async();

            return Results.Ok(result);
        })
        .WithName("SeedMilestone3")
        .WithSummary("Seeds 500 domain records, 50 simulated users, and 1,000 interactions.");

        // POST /api/dev/seed/final
        //
        // This creates the final-project scale dataset:
        // - 5,000 simulated domain records
        // - 500 simulated users
        // - 10,000+ interaction records across collections, comments, reactions, and activity
        group.MapPost("/final", async (SeedService seedService) =>
        {
            var result = await seedService.SeedFinalProjectAsync();

            return Results.Ok(result);
        })
        .WithName("SeedFinalProject")
        .WithSummary("Seeds the final-project scale dataset for local development demos.");

        return group;
    }
}