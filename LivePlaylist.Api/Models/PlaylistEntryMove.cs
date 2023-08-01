namespace LivePlaylist.Api.Models;

public class PlaylistEntryMove
{
    public Guid EntryId { get; set; } = Guid.Empty;
    
    public int Index { get; set; }
}
