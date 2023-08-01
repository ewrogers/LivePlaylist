using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistChangesValidator : AbstractValidator<PlaylistChanges>
{
    public PlaylistChangesValidator()
    {
        // Unless the action is clear, the song id collection must not be empty
        RuleFor(m => m.SongIds)
            .NotEmpty()
            .When(m => m.Action != PlaylistChangeType.Clear);

        // The index can only be set when the action is add to act as an insert
        RuleFor(m => m.Index)
            .Null()
            .When(m => m.Action != PlaylistChangeType.Add);

        // The index must be a positive integer value, when it is set
        RuleFor(m => m.Index)
            .GreaterThanOrEqualTo(0)
            .When(m => m.Index is not null);
    }
}
