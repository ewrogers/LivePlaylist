using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public interface IPlaylistService
{
    Task<IEnumerable<Playlist>> GetAllAsync();
    
    Task<Playlist?> GetByIdAsync(Guid id);
    
    Task<bool> CreateAsync(Playlist playlist);
    
    Task<bool> UpdateAsync(Playlist playlist);
    
    Task<bool> DeleteAsync(Guid id);
}
