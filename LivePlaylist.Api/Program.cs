using FluentValidation;
using LivePlaylist.Api.Data;
using LivePlaylist.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Adds all service dependencies for endpoints in the assembly
builder.Services.AddEndpoints<Program>(builder.Configuration);

// Adds all validators for models in the assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Enable OpenAPI/Swagger document generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataInitializer>();

var app = builder.Build();

// Mount the Swagger UI at /swagger/index.html
app.UseSwagger();
app.UseSwaggerUI();

// Map all endpoints in the assembly
app.UseEndpoints<Program>();

// Seed initial data
var dataInitializer = app.Services.GetRequiredService<DataInitializer>();
await dataInitializer.InitializeAsync();

app.Run();
