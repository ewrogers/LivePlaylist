using System.Text;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Data;

public class DataInitializer
{
    private const string SongsFile = "songs.csv";

    private readonly IUserService _userService;
    private readonly IPlaylistService _playlistService;
    private readonly ISongService _songService;
    private readonly IConfiguration _configuration;

    public DataInitializer(
        IUserService userService,
        IPlaylistService playlistService,
        ISongService songService,
        IConfiguration configuration)
    {
        _userService = userService;
        _playlistService = playlistService;
        _songService = songService;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        var songRows = await ReadSongsFromCsvFile(SongsFile);

        foreach (var song in songRows)
        {
            await _songService.CreateAsync(new Song
            {
                Artist = song.Artist,
                Title = song.Title
            });   
        }
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
