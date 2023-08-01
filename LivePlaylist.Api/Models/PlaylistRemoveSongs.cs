using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public class PlaylistRemoveSongs
{
    [Required]
    public ICollection<Guid> EntryIds { get; set; } = default!;
}
