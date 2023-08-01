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

    public Task AddSongsAsync(Playlist playlist, IEnumerable<Song> songs)
        => InsertSongsAsync(playlist, playlist.Entries.Count, songs);

    public Task InsertSongsAsync(Playlist playlist, int index, IEnumerable<Song> songs)
    {
        var newEntries = songs.Select(s => new PlaylistEntry { Song = s });
        playlist.Entries.InsertRange(index, newEntries);

        // Remove songs from the beginning of the playlist if it exceeds the max
        // This makes the playlist act like a FIFO queue
        while (playlist.Entries.Count is > 0 and Playlist.MaxEntries)
            playlist.Entries.RemoveAt(0);

        return Task.CompletedTask;
    }
    
    public Task RemoveSongsAsync(Playlist playlist, IEnumerable<Guid> entryIds)
    {
        playlist.Entries.RemoveAll(entry => entryIds.Contains(entry.EntryId));
        return Task.CompletedTask;
    }
    
    public Task ClearSongsAsync(Playlist playlist)
    {
        playlist.Entries.Clear();
        return Task.CompletedTask;
    }
}
