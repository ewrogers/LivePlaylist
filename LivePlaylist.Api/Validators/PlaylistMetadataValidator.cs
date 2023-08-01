using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistMetadataValidator : AbstractValidator<Playlist>
{
    public PlaylistMetadataValidator()
    {
        RuleFor(m => m.Name).NotEmpty();
    }
}
