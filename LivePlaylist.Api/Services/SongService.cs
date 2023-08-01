using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public class SongService : ISongService
{
    private readonly Dictionary<Guid, Song> _songs = new();
    
    public Task<IEnumerable<Song>> GetAllAsync()
    {
        var allSongs = _songs.Values.ToList();
        return Task.FromResult(allSongs.AsEnumerable());
    }

    public Task<IEnumerable<Song>> FindAsync(Func<Song, bool> predicate)
    {
        var matchingSongs = _songs.Values.Where(predicate).ToList();
        return Task.FromResult(matchingSongs.AsEnumerable());
    }
    
    public Task<Song?> FindOneAsync(Func<Song, bool> predicate)
    {
        var matchingSong = _songs.Values.FirstOrDefault(predicate);
        return Task.FromResult(matchingSong);
    }

    public Task<Song?> GetByIdAsync(Guid id)
    {
        _songs.TryGetValue(id, out var song);
        return Task.FromResult(song);
    }

    public Task<bool> CreateAsync(Song song)
    {
        if (_songs.ContainsKey(song.Id))
            return Task.FromResult(false);

        _songs.Add(song.Id, song);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(Song song)
    {
        if (!_songs.ContainsKey(song.Id))
            return Task.FromResult(false);
        
        _songs[song.Id] = song;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var wasRemoved = _songs.Remove(id);
        return Task.FromResult(wasRemoved);
    }
}
