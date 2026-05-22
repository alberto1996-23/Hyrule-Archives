using HyruleArchives.Data;
using HyruleArchives.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;

public static class RegionEndpoints
{
    public static RouteGroupBuilder MapRegionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/regions")
            .WithTags("Regions");

        group.MapGet("/", async (AppDbContext dbContext) =>
        {
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