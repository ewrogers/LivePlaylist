using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Api.Models;

public class PlaylistMetadata
{
    [Required]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
