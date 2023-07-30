using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Services;

public class UserService : IUserService
{
    // In-memory storage of users by username (case-insensitive)
    private readonly Dictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public Task<IEnumerable<User>> GetAllAsync()
    {
        var allUsers = _users.Values.ToList();
        return Task.FromResult(allUsers.AsEnumerable());
    }
    
    public Task<User?> GetByNameAsync(string username)
    {
        _users.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }

    public Task<bool> CreateAsync(User user)
    {
        if (_users.ContainsKey(user.Username))
            return Task.FromResult(false);
        
        _users.Add(user.Username, user);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(User user)
    {
        if (!_users.ContainsKey(user.Username))
            return Task.FromResult(false);
        
        _users[user.Username] = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string username)
    {
        var wasRemoved = _users.Remove(username);
        return Task.FromResult(wasRemoved);
    }
}
