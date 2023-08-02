using System.Text.Json.Serialization;

namespace LivePlaylist.Common.Models;

public class PlaylistEntry
{
    public Guid EntryId { get; set; } = Guid.NewGuid();
    
    [JsonIgnore]
    public Song Song { get; set; } = default!;

    public Guid SongId => Song.Id;
    public string Artist => Song.Artist;
    public string Title => Song.Title;
}
