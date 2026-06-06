using HyruleArchives.Data;
using HyruleArchives.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to expose release-region reference data for client screens.

// Region endpoints expose where releases happened, such as Japan or Global.
public static class RegionEndpoints
{
    public static RouteGroupBuilder MapRegionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/regions")
            .WithTags("Regions");

        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            // Regions are public metadata, so this endpoint does not require login.
            var regions = await dbContext.Regions
                .OrderBy(region => region.Name)
                .Select(region => new RegionDto(
                    region.Id,
                    region.Name,
                    region.Code
                ))
                .ToListAsync();

            return Results.Ok(regions);
        })
        .WithName("GetRegions")
        .WithSummary("Gets all release regions used in the archive.");

        return group;
    }
}