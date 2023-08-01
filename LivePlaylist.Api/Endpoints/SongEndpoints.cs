using LivePlaylist.Api.Filters;
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
            .Produces(401)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id:guid}}", GetSongById)
            .WithName(nameof(GetSongById))
            .Produces<Song>(200, ContentType)
            .Produces(401)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPost($"{BaseRoute}", CreateSong)
            .AddEndpointFilter<ValidationFilter<SongMetadata>>()
            .WithName(nameof(CreateSong))
            .Produces<Song>(201, ContentType)
            .Produces(400)
            .Produces(401)
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

    private static async Task<IResult> CreateSong(ISongService songService, SongMetadata fields)
    {
        var existingSong = await songService.FindOneAsync(s =>
            s.Artist.Equals(fields.Artist, StringComparison.OrdinalIgnoreCase) &&
            s.Title.Equals(fields.Title, StringComparison.OrdinalIgnoreCase));

        // If a song with the same artist and title already exists, return a 400
        if (existingSong is not null)
        {
            return Results.BadRequest(new
            {
                ErrorMessage = "A song with this artist and title already exists"
            });
        }
        
        var song = new Song
        {
            Artist = fields.Artist,
            Title = fields.Title
        };
        
        await songService.CreateAsync(song);
        return Results.CreatedAtRoute(nameof(GetSongById), new { song.Id }, song);
    }
}
