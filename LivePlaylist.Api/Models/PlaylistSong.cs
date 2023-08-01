using System.Text.Json.Serialization;

namespace LivePlaylist.Api.Models;

public class PlaylistSong
{
    public Guid EntryId { get; set; } = Guid.NewGuid();
    
    [JsonIgnore]
    public Song Song { get; set; } = default!;

    public Guid SongId => Song.Id;
    public string Artist => Song.Artist;
    public string Title => Song.Title;
}
