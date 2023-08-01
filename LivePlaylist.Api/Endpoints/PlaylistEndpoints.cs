using System.Security.Claims;
using FluentValidation;
using LivePlaylist.Api.Filters;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class PlaylistEndpoints : IEndpoints
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
            .Produces(404)
            .WithTags(Tag);

        app.MapPost($"{BaseRoute}", CreatePlaylist)
            .AddEndpointFilter<ValidationFilter<Playlist>>()
            .WithName(nameof(CreatePlaylist))
            .Produces<Playlist>(201, ContentType)
            .Produces(400)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id:guid}}", UpdatePlaylist)
            .AddEndpointFilter<ValidationFilter<Playlist>>()
            .WithName(nameof(UpdatePlaylist))
            .Produces<Playlist>(200, ContentType)
            .Produces(400)
            .Produces(403)
            .Produces(404)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id:guid}}", DeletePlaylist)
            .WithName(nameof(DeletePlaylist))
            .Produces(204)
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
        Playlist playlist,
        IPlaylistService playlistService,
        IValidator<Playlist> validator,
        ClaimsPrincipal user)
    {
        var username = user.Identity!.Name!;
        playlist.Owner = username;

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
        Playlist playlist, 
        IPlaylistService playlistService, 
        IValidator<Playlist> validator,
        ClaimsPrincipal user)
    {
        var username = user.Identity!.Name!;
        
        // Set the playlist ID to the ID from the route param
        playlist.Id = id;
        
        // If the playlist does not exist, return a 404
        var existingPlaylist = await playlistService.GetByIdAsync(playlist.Id);
        if (existingPlaylist is null)
        {
            return Results.NotFound();
        }

        // If the current user is not the owner of the playlist, return a 403
        if (!string.Equals(existingPlaylist.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }

        var wasUpdated = await playlistService.UpdateAsync(playlist);
        return wasUpdated ? Results.Ok(playlist) : Results.NotFound();
    }

    private static async Task<IResult> DeletePlaylist(
        Guid id,
        IPlaylistService playlistService,
        ClaimsPrincipal user)
    {
        var username = user.Identity!.Name!;
        
        // If the playlist does not exist, return a 404
        var existingPlaylist = await playlistService.GetByIdAsync(id);
        if (existingPlaylist is null)
        {
            return Results.NotFound();
        }
        
        // If the current user is not the owner of the playlist, return a 403
        if (!string.Equals(existingPlaylist.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }

        await playlistService.DeleteAsync(id);
        return Results.NoContent();
    }
}
