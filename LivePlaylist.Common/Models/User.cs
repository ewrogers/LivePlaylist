using System.ComponentModel.DataAnnotations;

namespace LivePlaylist.Common.Models;

public class User
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string DisplayName { get; set; } = default!;
}
