using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public class PlaylistAddSongs
{
    public int? Index { get; set; }
    
    [Required]
    public ICollection<Guid> SongIds { get; set; } = default!;
}
