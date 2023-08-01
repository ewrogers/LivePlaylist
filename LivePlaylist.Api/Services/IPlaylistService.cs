using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public interface IPlaylistService
{
    Task<IEnumerable<Playlist>> GetAllAsync();
    
    Task<Playlist?> GetByIdAsync(Guid id);
    
    Task<bool> CreateAsync(Playlist playlist);
    
    Task<bool> UpdateAsync(Playlist playlist);
    
    Task<bool> DeleteAsync(Guid id);

    Task AddSongsAsync(Playlist playlist, IEnumerable<Song> songs);
    
    Task InsertSongsAsync(Playlist playlist, int index, IEnumerable<Song> songs);
    
    Task RemoveSongsAsync(Playlist playlist, IEnumerable<Guid> entryIds);
    
    Task ClearSongsAsync(Playlist playlist);
}
