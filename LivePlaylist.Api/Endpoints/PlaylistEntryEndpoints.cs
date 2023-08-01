using System.Security.Claims;
using LivePlaylist.Api.Filters;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class PlaylistEntryEndpoints : IEndpoints
{
    private const string BaseRoute = "playlists/{id:guid}";
    private const string ContentType = "application/json";
    private const string Tag = "Playlist Management";

    public static void AddServices(IServiceCollection services, IConfiguration configuration) { }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}/songs", GetPlaylistSongs)
            .WithName(nameof(GetPlaylistSongs))
            .Produces<IEnumerable<PlaylistEntry>>(200, ContentType)
            .Produces(404)
            .WithTags(Tag);

        app.MapPost($"{BaseRoute}/add-songs", AddOrInsertPlaylistSongs)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<PlaylistEntryAdd>>()
            .WithName(nameof(AddOrInsertPlaylistSongs))
            .Produces<IEnumerable<PlaylistEntry>>(200, ContentType)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPost($"{BaseRoute}/move-song", MovePlaylistSong)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<PlaylistEntryMove>>()
            .WithName(nameof(MovePlaylistSong))
            .Produces<IEnumerable<PlaylistEntry>>(200, ContentType)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPost($"{BaseRoute}/remove-songs", RemovePlaylistSongs)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<PlaylistEntryRemove>>()
            .WithName(nameof(RemovePlaylistSongs))
            .Produces<IEnumerable<PlaylistEntry>>(200, ContentType)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);
    }

    private static async Task<IResult> GetPlaylistSongs(
        Guid id,
        IPlaylistService playlistService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        return playlist is null ? Results.NotFound() : Results.Ok(playlist.Entries);
    }

    private static async Task<IResult> AddOrInsertPlaylistSongs(
        Guid id,
        PlaylistEntryAdd changes,
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
        
        // If the index is null, add the songs to the end of the playlist
        var startIndex = changes.Index ?? playlist.Entries.Count;
        
        // Find the songs to add from the song library (if they exist)
        var songsToAdd = await songService.FindAsync(s => changes.SongIds.Contains(s.Id));

        // Insert or add the songs to the playlist based on the index
        if (startIndex < playlist.Entries.Count)
            await playlistService.InsertSongsAsync(playlist, startIndex, songsToAdd);
        else
            await playlistService.AddSongsAsync(playlist, songsToAdd);
        
        return Results.Ok(playlist.Entries);
    }

    private static async Task<IResult> MovePlaylistSong(
        Guid id,
        PlaylistEntryMove changes,
        IPlaylistService playlistService,
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
        
        await playlistService.MoveSongAsync(playlist, changes.EntryId, changes.Index);
        return Results.Ok(playlist.Entries);
    }

    private static async Task<IResult> RemovePlaylistSongs(
        Guid id,
        PlaylistEntryRemove changes,
        IPlaylistService playlistService,
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

        await playlistService.RemoveSongsAsync(playlist, changes.EntryIds);
        return Results.Ok(playlist.Entries);
    }
}
