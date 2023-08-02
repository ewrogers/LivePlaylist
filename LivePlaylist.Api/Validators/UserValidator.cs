using System.Text.RegularExpressions;
using FluentValidation;
using LivePlaylist.Common.Models;

namespace LivePlaylist.Api.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(m => m.Username)
            .NotEmpty()
            .Matches("^[a-z0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        RuleFor(m => m.DisplayName).NotEmpty();
    }
}
