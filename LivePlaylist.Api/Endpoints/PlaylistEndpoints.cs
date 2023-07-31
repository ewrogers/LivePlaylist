using FluentValidation;
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
            .WithName(nameof(CreatePlaylist))
            .Produces<Playlist>(201, ContentType)
            .Produces(400)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id:guid}}", UpdatePlaylist)
            .WithName(nameof(UpdatePlaylist))
            .Produces<Playlist>(200, ContentType)
            .Produces(400)
            .Produces(404)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id:guid}}", DeletePlaylist)
            .WithName(nameof(DeletePlaylist))
            .Produces(204)
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
        IValidator<Playlist> validator)
    {
        // TODO: assign the current user as the owner of the playlist

        var validationResult = await validator.ValidateAsync(playlist);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        if (!await playlistService.CreateAsync(playlist))
        {
            return Results.BadRequest();
        }

        return Results.CreatedAtRoute(nameof(GetPlaylistById), new { playlist.Id }, playlist);
    }

    private static async Task<IResult> UpdatePlaylist(
        Guid id,
        Playlist playlist, 
        IPlaylistService playlistService, 
        IValidator<Playlist> validator)
    {
        // Set the playlist ID to the ID from the route param
        playlist.Id = id;
        
        var validationResult = await validator.ValidateAsync(playlist);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }
        
        var existingPlaylist = await playlistService.GetByIdAsync(playlist.Id);
        if (existingPlaylist is null)
        {
            return Results.NotFound();
        }

        // TODO: Check if the current user is the owner of the playlist
        
        var wasUpdated = await playlistService.UpdateAsync(playlist);
        return wasUpdated ? Results.Ok(playlist) : Results.NotFound();
    }

    private static async Task<IResult> DeletePlaylist(Guid id, IPlaylistService playlistService)
    {
        var playlist = await playlistService.GetByIdAsync(id);
        if (playlist is null)
        {
            return Results.NotFound();
        }
        
        // TODO: Check if the current user is the owner of the playlist

        await playlistService.DeleteAsync(id);
        return Results.NoContent();
    }
}
