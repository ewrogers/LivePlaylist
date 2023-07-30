using FluentValidation;
using LivePlaylist.Api.Models;
using LivePlaylist.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Seed in-memory database here

// Mount endpoints
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

app.Run();
