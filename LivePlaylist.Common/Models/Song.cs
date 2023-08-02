
namespace LivePlaylist.Common.Models;

public class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Artist { get; set; } = default!;
    
    public string Title { get; set; } = default!;
}
