using System.Security.Claims;
using LivePlaylist.Api.Filters;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class PlaylistSongEndpoints : IEndpoints
{
    private const string BaseRoute = "playlists/{id:guid}";
    private const string ContentType = "application/json";
    private const string Tag = "Playlist Songs";

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}/songs", GetPlaylistSongs)
            .WithName(nameof(GetPlaylistSongs))
            .Produces<IEnumerable<Song>>(200, ContentType)
            .Produces(404)
            .Produces(401)
            .WithTags(Tag);

        app.MapPost($"{BaseRoute}/songs", ApplyPlaylistChanges)
            .AddEndpointFilter<ValidationFilter<PlaylistChanges>>()
            .WithName(nameof(ApplyPlaylistChanges))
            .Produces<IEnumerable<Song>>(200, ContentType)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);
    }

    private static async Task<IResult> GetPlaylistSongs(
        Guid id,
        PlaylistChanges changes,
        IPlaylistService playlistService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        return playlist is null ? Results.NotFound() : Results.Ok(playlist.Songs);
    }

    private static async Task<IResult> ApplyPlaylistChanges(
        Guid id,
        PlaylistChanges changes,
        IPlaylistService playlistService,
        ISongService songService,
        ClaimsPrincipal user)
    {
        var username = user.Identity!.Name!;
        
        // If the playlist does not exist, return a 404
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist is null)
        {
            return Results.NotFound();
        }
        
        // If the current user is not the owner of the playlist, return a 403
        if (!string.Equals(playlist.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }

        // Perform different mutations based on the change action
        switch (changes.Action)
        {
            case PlaylistChangeType.Add:
                // If the index is null, add the songs to the end of the playlist
                var startIndex = changes.Index ?? playlist.Songs.Count;
                
                // Find the songs to add from the song library (if they exist)
                var songsToAdd = await songService.FindAsync(s => changes.SongIds.Contains(s.Id));

                // Insert or add the songs to the playlist based on the index
                if (startIndex < playlist.Songs.Count)
                    await playlistService.InsertSongsAsync(playlist, startIndex, songsToAdd);
                else
                    await playlistService.AddSongsAsync(playlist, songsToAdd);

                break;
            
            case PlaylistChangeType.Remove:
                await playlistService.RemoveSongsAsync(playlist, changes.SongIds);
                break;
            
            case PlaylistChangeType.Clear:
                await playlistService.ClearSongsAsync(playlist);
                break;
            
            default:
                return Results.BadRequest(new
                {
                    ErrorMessage = "Unsupported change action"
                });
        }

        await playlistService.UpdateAsync(playlist);
        return Results.Ok(playlist.Songs);
    }
}
