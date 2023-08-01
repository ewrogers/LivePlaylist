using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Filters;

public class PlaylistOwnerFilter : IEndpointFilter
{
    private readonly IPlaylistService _playlistService;

    public PlaylistOwnerFilter(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Attempt to find a Guid in the arguments, which should be the playlist ID
        // If more than one Guid is found, return a 400 as well
        var idParam = context.Arguments.SingleOrDefault(arg => arg?.GetType() == typeof(Guid));
        if (idParam is not Guid playlistId)
        {
            return Results.BadRequest();
        }

        // If the user is not authenticated, return a 401
        var username = context.HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return Results.Unauthorized();
        }

        // If the playlist does not exist, return a 404
        var playlist = await _playlistService.GetByIdAsync(playlistId);
        if (playlist is null)
        {
            return Results.NotFound();
        }
        
        // If the current user is not the owner of the playlist, return a 403
        if (!string.Equals(playlist.Owner, username, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }
        
        var result = await next.Invoke(context);
        return result;
    }
}
