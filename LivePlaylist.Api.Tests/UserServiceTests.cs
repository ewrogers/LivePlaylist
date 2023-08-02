
using FluentAssertions;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Tests;

public class UserServiceTests
{
    private readonly UserService _userService;

    public UserServiceTests()
    {
        // Create a new instance of the service before each test
        _userService = new UserService();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new[]
        {
            GenerateUser("user-1", "User 1"),
            GenerateUser("user-2", "User 2")
        };

        foreach (var user in users)
            await _userService.CreateAsync(user);

        // Act
        var results = await _userService.GetAllAsync();

        // Assert
        results.Should().BeEquivalentTo(users);
    }
    
    [Fact]
    public async Task GetByNameAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        var result = await _userService.GetByNameAsync(user.Username);

        // Assert
        result.Should().Be(user);
    }
    
    [Fact]
    public async Task GetByNameAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userService.GetByNameAsync("non-existent-user");

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenUserDoesNotExist()
    {
        // Arrange
        var user = GenerateUser();

        // Act
        var result = await _userService.CreateAsync(user);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenUserExists()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        var result = await _userService.CreateAsync(user);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateAsync_AddsUserToStorage_WhenUserDoesNotExist()
    {
        // Arrange
        var user = GenerateUser();

        // Act
        await _userService.CreateAsync(user);

        // Assert
        var result = await _userService.GetByNameAsync(user.Username);
        result.Should().Be(user);
    }
    
    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        var result = await _userService.UpdateAsync(user);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var user = GenerateUser();

        // Act
        var result = await _userService.UpdateAsync(user);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task UpdateAsync_UpdatesUserInStorage_WhenUserExists()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        var updatedUser = GenerateUser(user.Username, "Updated User");
        await _userService.UpdateAsync(updatedUser);

        // Assert
        var result = await _userService.GetByNameAsync(user.Username);
        result.Should().Be(updatedUser);
    }
    
    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenUserWasRemoved()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        var result = await _userService.DeleteAsync(user.Username);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userService.DeleteAsync("non-existent-user");

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteAsync_RemovesUserFromStorage_WhenUserExists()
    {
        // Arrange
        var user = GenerateUser();
        await _userService.CreateAsync(user);

        // Act
        await _userService.DeleteAsync(user.Username);

        // Assert
        var result = await _userService.GetByNameAsync(user.Username);
        result.Should().BeNull();
    }

    private static User GenerateUser(string username = "test-user", string displayName = "Test User")
    {
        return new User
        {
            Username = username,
            DisplayName = displayName
        };
    }
}
