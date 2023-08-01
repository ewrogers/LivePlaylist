using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class PlaylistEntryRemoveValidator : AbstractValidator<PlaylistEntryRemove>
{
    public PlaylistEntryRemoveValidator()
    {
        // Entry ID collection is required and must not be empty
        RuleFor(m => m.EntryIds).NotEmpty();
    }
}
