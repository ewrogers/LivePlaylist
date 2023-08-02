namespace LivePlaylist.Common.Models;

public class Playlist
{
    public const int MaxEntries = 200;
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Owner { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public List<PlaylistEntry> Entries { get; set; } = new();
}
