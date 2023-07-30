using FluentValidation;
using LivePlaylist.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Adds all service dependencies for endpoints in the assembly
builder.Services.AddEndpoints<Program>(builder.Configuration);

// Adds all validators for models in the assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Enable OpenAPI/Swagger document generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Mount the Swagger UI at /swagger/index.html
app.UseSwagger();
app.UseSwaggerUI();

// Map all endpoints in the assembly
app.UseEndpoints<Program>();

// TODO: Seed in-memory database here

app.Run();
