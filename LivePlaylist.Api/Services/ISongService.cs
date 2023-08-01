using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public interface ISongService
{
    Task<IEnumerable<Song>> GetAllAsync();
    
    Task<IEnumerable<Song>> FindAsync(Func<Song, bool> predicate);
    
    Task<Song?> FindOneAsync(Func<Song, bool> predicate);
    
    Task<Song?> GetByIdAsync(Guid id);
    
    Task<bool> CreateAsync(Song song);
    
    Task<bool> UpdateAsync(Song song);
    
    Task<bool> DeleteAsync(Guid id);
}
