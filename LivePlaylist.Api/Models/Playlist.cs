namespace LivePlaylist.Api.Models;

public class Playlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Owner { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
}
