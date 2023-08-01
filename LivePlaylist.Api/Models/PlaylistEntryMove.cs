using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public class PlaylistEntryMove
{
    [Required]
    public Guid EntryId { get; set; } = Guid.Empty;
    
    [Required]
    public int Index { get; set; }
}
