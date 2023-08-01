using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistAddSongsValidator : AbstractValidator<PlaylistAddSongs>
{
    public PlaylistAddSongsValidator()
    {
        // Song ID collection is required and must not be empty
        RuleFor(m => m.SongIds)
            .NotEmpty();

        // Index is optional, but if it's provided, it must be greater than or equal to 0
        RuleFor(m => m.Index).GreaterThanOrEqualTo(0)
            .When(m => m.Index.HasValue);
    }
}
