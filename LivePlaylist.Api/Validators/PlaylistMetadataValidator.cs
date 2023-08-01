using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistMetadataValidator : AbstractValidator<PlaylistMetadata>
{
    public PlaylistMetadataValidator()
    {
        RuleFor(m => m.Name).NotEmpty();
    }
}
