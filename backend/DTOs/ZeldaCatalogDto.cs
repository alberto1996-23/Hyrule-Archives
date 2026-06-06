namespace HyruleArchives.DTOs;

// I use these DTOs to send the curated Zelda catalog to the frontend without exposing EF entities.

public record ZeldaCatalogAvailabilityDto(
    string Name,
    string ImagePath,
    string? Note
);

public record ZeldaCatalogCollectionAppearanceDto(
    string Title,
    string BoxArtPath,
    ZeldaCatalogAvailabilityDto Platform
);

public record ZeldaCatalogGameDto(
    string Slug,
    int MobyGameId,
    string Title,
    string BoxArtPath,
    ZeldaCatalogAvailabilityDto PrimaryConsole,
    List<ZeldaCatalogAvailabilityDto> SecondaryConsoles,
    List<ZeldaCatalogAvailabilityDto> DownloadedPorts,
    List<ZeldaCatalogAvailabilityDto> SubscriptionServices,
    List<ZeldaCatalogCollectionAppearanceDto> CollectionAppearances
);
