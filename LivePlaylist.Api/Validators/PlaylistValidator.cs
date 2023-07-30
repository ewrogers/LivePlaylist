using System.Text.RegularExpressions;
using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistValidator : AbstractValidator<Playlist>
{
    public PlaylistValidator()
    {
        RuleFor(m => m.Owner)
            .NotEmpty()
            .Matches("^[a-z0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        RuleFor(m => m.Name).NotEmpty();
        RuleFor(m => m.Description).NotNull();
    }
}
