using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class SongEndpoints : IEndpoints
{
    private const string BaseRoute = "songs";
    private const string ContentType = "application/json";
    private const string Tag = "Songs";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISongService, SongService>();
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}", SearchSongs)
            .WithName(nameof(SearchSongs))
            .Produces<IEnumerable<Song>>(200, ContentType)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id:guid}}", GetSongById)
            .WithName(nameof(GetSongById))
            .Produces<Song>(200, ContentType)
            .Produces(404)
            .WithTags(Tag);
    }

    private static async Task<IResult> SearchSongs(string? searchTerm, ISongService songService)
    {
        // If the search term is null or whitespace, return all songs
        // If the search term is not null or whitespace, return only the songs that match the search term
        var songs = string.IsNullOrWhiteSpace(searchTerm)
            ? await songService.GetAllAsync()
            : await songService.FindAsync(s =>
                s.Artist.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        return Results.Ok(songs);
    }

    private static async Task<IResult> GetSongById(ISongService songService, Guid id)
    {
        var song = await songService.GetByIdAsync(id);
        return song is null ? Results.NotFound() : Results.Ok(song);
    }
}
