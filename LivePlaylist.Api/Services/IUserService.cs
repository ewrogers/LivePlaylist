using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public interface IUserService
{
    Task<bool> CreateAsync(User user);
    
    Task<User?> GetByNameAsync(string username);
    
    Task<IEnumerable<User>> GetAllAsync();

    Task<bool> UpdateAsync(User user);
    
    Task<bool> DeleteAsync(string username);
}
