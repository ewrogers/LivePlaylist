using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class SongMetadataValidator : AbstractValidator<SongMetadata>
{
    public SongMetadataValidator()
    {
        RuleFor(m => m.Artist).NotEmpty();
        RuleFor(m => m.Title).NotEmpty();
    }
}
