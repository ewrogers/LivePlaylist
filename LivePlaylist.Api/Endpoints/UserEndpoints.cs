using FluentValidation;
using LivePlaylist.Api.Filters;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class UserEndpoints : IEndpoints
{
    private const string BaseRoute = "users";
    private const string ContentType = "application/json";
    private const string Tag = "Users";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserService, UserService>();
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}", GetAllUsers)
            .WithName(nameof(GetAllUsers))
            .Produces<IEnumerable<User>>(200, ContentType)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{username:regex(^[a-zA-Z0-9]+$)}}", GetUserByName)
            .WithName(nameof(GetUserByName))
            .Produces<User>(200, ContentType)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPost($"{BaseRoute}", CreateUser)
            .AddEndpointFilter<ValidationFilter<User>>()
            .WithName(nameof(CreateUser))
            .Produces<User>(200, ContentType)
            .Produces(400)
            .WithTags(Tag);
    }
    
    private static async Task<IResult> GetAllUsers(IUserService userService)
    {
        var users = await userService.GetAllAsync();
        return Results.Ok(users);
    }
    
    private static async Task<IResult> GetUserByName(string username, IUserService userService)
    {
        var user = await userService.GetByNameAsync(username);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }
    
    private static async Task<IResult> CreateUser(
        User user, 
        IUserService userService, 
        IValidator<User> validator)
    {
        var wasCreated = await userService.CreateAsync(user);
        if (!wasCreated)
        {
            return Results.BadRequest(new
            {
                ErrorMessage = "User with that username already exists"
            });
        }

        return Results.CreatedAtRoute(nameof(GetUserByName), new { user.Username }, user);
    }
}
