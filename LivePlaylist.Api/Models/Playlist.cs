namespace LivePlaylist.Api.Models;

public class Playlist
{
    public const int MaxSongs = 200;
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Owner { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public List<Song> Songs { get; set; } = new();
}
