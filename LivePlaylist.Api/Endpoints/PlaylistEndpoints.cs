using System.Security.Claims;
using LivePlaylist.Api.Filters;
using LivePlaylist.Api.Services;
using LivePlaylist.Common.Models;

namespace LivePlaylist.Api.Endpoints;

public class PlaylistEndpoints : IEndpointCollection
{
    private const string BaseRoute = "playlists";
    private const string ContentType = "application/json";
    private const string Tag = "Playlists";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPlaylistService, PlaylistService>();
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}", GetAllPlaylists)
            .WithName(nameof(GetAllPlaylists))
            .Produces<IEnumerable<Playlist>>(200, ContentType)
            .WithTags(Tag);

        app.MapGet($"{BaseRoute}/{{id:guid}}", GetPlaylistById)
            .WithName(nameof(GetPlaylistById))
            .Produces<Playlist>(200, ContentType)
            .Produces(401)
            .Produces(404)
            .WithTags(Tag);

        app.MapPost($"{BaseRoute}", CreatePlaylist)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<PlaylistMetadata>>()
            .WithName(nameof(CreatePlaylist))
            .Produces<Playlist>(201, ContentType)
            .Produces(401)
            .Produces(400)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id:guid}}", UpdatePlaylist)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<PlaylistMetadata>>()
            .AddEndpointFilter<PlaylistOwnerFilter>()
            .WithName(nameof(UpdatePlaylist))
            .Produces<Playlist>(200, ContentType)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id:guid}}", DeletePlaylist)
            .RequireAuthorization()
            .AddEndpointFilter<PlaylistOwnerFilter>()
            .WithName(nameof(DeletePlaylist))
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);
    }
    
    private static async Task<IResult> GetAllPlaylists(IPlaylistService playlistService)
    {
        var playlists = await playlistService.GetAllAsync();
        return Results.Ok(playlists);
    }
    
    private static async Task<IResult> GetPlaylistById(Guid id, IPlaylistService playlistService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        return playlist is null ? Results.NotFound() : Results.Ok(playlist);
    }

    private static async Task<IResult> CreatePlaylist(
        PlaylistMetadata fields,
        IPlaylistService playlistService,
        ClaimsPrincipal user)
    {
        // If the user is not authenticated, return a 401
        var username = user.Identity?.Name;
        if (username is null)
        {
            return Results.Unauthorized();
        }
        
        var playlist = new Playlist
        {
            Owner = username,
            Name = fields.Name,
            Description = fields.Description ?? string.Empty
        };

        // If the playlist could not be created, return a 400
        if (!await playlistService.CreateAsync(playlist))
        {
            return Results.BadRequest(new
            {
                ErrorMessage = "Playlist could not be created"
            });
        }

        return Results.CreatedAtRoute(nameof(GetPlaylistById), new { playlist.Id }, playlist);
    }

    private static async Task<IResult> UpdatePlaylist(
        Guid id,
        PlaylistMetadata changes,
        IPlaylistService playlistService)
    {
        // If the playlist does not exist, return a 404
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist is null)
            return Results.NotFound();
        
        playlist.Name = changes.Name;
        playlist.Description = changes.Description ?? playlist.Description;

        var wasUpdated = await playlistService.UpdateAsync(playlist);
        return wasUpdated ? Results.Ok(playlist) : Results.NotFound();
    }

    private static async Task<IResult> DeletePlaylist(
        Guid id,
        IPlaylistService playlistService)
    {
        await playlistService.DeleteAsync(id);
        return Results.NoContent();
    }
}
