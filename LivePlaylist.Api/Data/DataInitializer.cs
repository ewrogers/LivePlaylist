using System.Text;
using LivePlaylist.Api.Services;
using LivePlaylist.Common.Models;

namespace LivePlaylist.Api.Data;

internal class DataInitializer
{
    private const string SongsFile = "songs.csv";

    private readonly IUserService _userService;
    private readonly IPlaylistService _playlistService;
    private readonly ISongService _songService;
    private readonly ILogger _logger;

    public DataInitializer(
        IUserService userService,
        IPlaylistService playlistService,
        ISongService songService,
        ILogger<Program> logger)
    {
        _userService = userService;
        _playlistService = playlistService;
        _songService = songService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {await InitializeUsers();
        await InitializeSongs();
    }

    private async Task InitializeUsers()
    {
        var adminUser = new User
        {
            Username = "admin",
            DisplayName = "Administrator"
        };

        await _userService.CreateAsync(adminUser);
        
        _logger.LogInformation("Created default user: {Username}", adminUser.Username);
    }

    private async Task InitializeSongs()
    {
        _logger.LogInformation("Loading songs from CSV file: {SongsFile}", SongsFile);
        var songRows = await ReadSongsFromCsvFile(SongsFile);

        var seedCount = 0;
        foreach (var song in songRows)
        {
            await _songService.CreateAsync(new Song
            {
                Artist = song.Artist,
                Title = song.Title
            });

            seedCount += 1;
        }
        
        _logger.LogInformation("Seeded {SeedCount} songs from CSV file: {SongsFile}", seedCount, SongsFile);
    }

    // Reads the songs from the CSV file and returns them as tuples (Title, Artist)
    private static async Task<IEnumerable<(string Title, string Artist)>> ReadSongsFromCsvFile(string filename)
    {
        await using var stream = new FileStream(SongsFile, FileMode.Open, FileAccess.Read);
        using var reader = new CsvReader(stream, Encoding.UTF8);

        var results = new List<(string Title, string Artist)>();
        
        // Skip the first line, which are the column headers
        var fields = await reader.ReadLineAsync();

        if (fields is null)
            return results;

        while (true)
        {
            fields = await reader.ReadLineAsync();
            if (fields is null)
                break;

            if (fields.Count != 2)
                throw new FormatException("One or more lines are invalid CSV format.");

            results.Add((fields[0], fields[1]));
        }

        return results;
    }
}
