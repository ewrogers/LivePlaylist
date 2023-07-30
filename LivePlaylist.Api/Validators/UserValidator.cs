using FluentValidation;
using LivePlaylist.Api.Models;

namespace LivePlaylist.Api.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(m => m.Username)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$");

        RuleFor(m => m.DisplayName).NotEmpty();
    }
}
