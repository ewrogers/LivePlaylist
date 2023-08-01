using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public enum PlaylistChangeType
{
    Add,
    Remove,
    Clear
}

public class PlaylistChanges
{
    [Required]
    public PlaylistChangeType Action { get; set; }
    
    public int? Index { get; set; }
    
    public ICollection<Guid> SongIds { get; set; } = default!;
}
