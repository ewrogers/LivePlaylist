using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public class PlaylistEntryRemove
{
    [Required]
    public ICollection<Guid> EntryIds { get; set; } = default!;
}
