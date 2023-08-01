using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using LivePlaylist.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace LivePlaylist.Api.Auth;

public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    private static readonly Regex ApiKeyRegex = new(@"^user\s+([a-z0-9]+)\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IUserService _userService;

    public ApiKeyAuthHandler(
        IUserService userService,
        IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Request.Headers;

        // Check if the Authorization header is present
        if (!headers.ContainsKey(HeaderNames.Authorization))
        {
            return AuthenticateResult.Fail("Missing API Key");
        }

        // Check if the Authorization header is valid and matches the expected format
        var header = Request.Headers[HeaderNames.Authorization].ToString();
        var match = ApiKeyRegex.Match(header);

        if (!match.Success)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        // Check that the user exists
        var username = match.Groups[1].Value;
        var user = await _userService.GetByNameAsync(username);

        if (user is null)
        {
            return AuthenticateResult.Fail("Invalid User");
        }

        // Build the claims identity for the user
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
