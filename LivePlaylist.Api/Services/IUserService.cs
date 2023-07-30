using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    
    Task<User?> GetByNameAsync(string username);
    
    Task<bool> CreateAsync(User user);
    
    Task<bool> UpdateAsync(User user);
    
    Task<bool> DeleteAsync(string username);
}
