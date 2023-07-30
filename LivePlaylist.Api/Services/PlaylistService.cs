using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public class PlaylistService : IPlaylistService
{
    // In-memory storage of playlists by id (guid)
    private readonly Dictionary<Guid, Playlist> _playlists = new ();

    public Task<IEnumerable<Playlist>> GetAllAsync()
    {
        var allPlaylists = _playlists.Values.ToList();
        return Task.FromResult(allPlaylists.AsEnumerable());
    }

    public Task<Playlist?> GetByIdAsync(Guid id)
    {
        _playlists.TryGetValue(id, out var playlist);
        return Task.FromResult(playlist);
    }

    public Task<bool> CreateAsync(Playlist playlist)
    {
        if (_playlists.ContainsKey(playlist.Id))
            return Task.FromResult(false);
        
        _playlists.Add(playlist.Id, playlist);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(Playlist playlist)
    {
        if (!_playlists.ContainsKey(playlist.Id))
            return Task.FromResult(false);
        
        _playlists[playlist.Id] = playlist;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var wasRemoved = _playlists.Remove(id);
        return Task.FromResult(wasRemoved);
    }
}
