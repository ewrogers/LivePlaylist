using FluentValidation;
using LivePlaylist.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints<Program>(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints<Program>();

// Seed in-memory database here

app.Run();
