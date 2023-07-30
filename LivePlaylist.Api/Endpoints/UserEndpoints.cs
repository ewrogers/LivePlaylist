using FluentValidation;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

namespace LivePlaylist.Api.Endpoints;

public class UserEndpoints : IEndpoints
{
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserService, UserService>();
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{username:regex(^[a-zA-Z0-9]+$)}",
            async (string username, IUserService userService) =>
            {
                var user = await userService.GetByNameAsync(username);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            }).WithName("GetUser");

        app.MapGet("users",
            async (IUserService userService) =>
            {
                var users = await userService.GetAllAsync();
                return Results.Ok(users);
            });

        app.MapPost("users", async (User user, IUserService userService, IValidator<User> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            } 
    
            var wasCreated = await userService.CreateAsync(user);
            if (!wasCreated)
            {
                return Results.BadRequest(new
                {
                    ErrorMessage = "User with that username already exists"
                });
            }

            return Results.CreatedAtRoute("GetUser", new { user.Username }, user);
        });
    }
}
