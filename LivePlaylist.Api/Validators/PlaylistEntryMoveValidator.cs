using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistEntryMoveValidator : AbstractValidator<PlaylistEntryMove>
{
    public PlaylistEntryMoveValidator()
    {
        // Entry ID is required and must not be empty
        RuleFor(m => m.EntryId).NotEmpty();

        // Index is required and must be greater than or equal to 0
        RuleFor(m => m.Index).GreaterThanOrEqualTo(0);
    }
}
