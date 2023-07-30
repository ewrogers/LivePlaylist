using LivePlaylist.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Seed in-memory database here

app.Run();
