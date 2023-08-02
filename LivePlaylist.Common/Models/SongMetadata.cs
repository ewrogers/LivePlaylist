using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Common.Models;

public class SongMetadata
{
    [Required]
    public string Artist { get; set; } = default!;
    
    [Required]
    public string Title { get; set; } = default!;
}
