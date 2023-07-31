using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class PlaylistSongEndpoints : IEndpoints
{
    private const string BaseRoute = "playlists/{id:guid}";
    private const string ContentType = "application/json";
    private const string Tag = "Playlist Songs";
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Do nothing
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}/songs", GetPlaylistSongs)
            .WithName(nameof(GetPlaylistSongs))
            .Produces<IEnumerable<Song>>(200, ContentType)
            .Produces(404)
            .WithTags(Tag);

        app.MapPost($"{BaseRoute}/songs/add", AddSongsToPlaylist)
            .WithName(nameof(AddSongsToPlaylist))
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .WithTags(Tag);
    }

    private static async Task<IResult> GetPlaylistSongs(Guid id, IPlaylistService playlistService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        return playlist is null ? Results.NotFound() : Results.Ok(playlist.Songs);
    }

    private static async Task<IResult> AddSongsToPlaylist(
        Guid id,
        IReadOnlyCollection<Guid> songIds,
        IPlaylistService playlistService,
        ISongService songService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist is null)
        {
            return Results.NotFound();
        }
        
        // TODO: check that the current user is the owner of the playlist

        var songs = await songService.FindAsync(s => songIds.Contains(s.Id));
        
        // TODO: add songs to playlist

        return Results.Ok();
    }
}
