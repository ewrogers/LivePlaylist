using FluentValidation;
using LivePlaylist.Api.Auth;
using LivePlaylist.Api.Data;
using LivePlaylist.Api.Endpoints;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adds all service dependencies for endpoints in the assembly
builder.Services.AddEndpoints<Program>(builder.Configuration);

// Adds all validators for models in the assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add authentication scheme (API key)
builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();

// Enable OpenAPI/Swagger document generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add security definition for API key to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid API key in the format of 'User {username}'",
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "User {username}",
        Scheme = "Bearer"
    });

    // Add security requirement for API key to all routes in Swagger UI
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    options.UseInlineDefinitionsForEnums();
});

builder.Services.AddSingleton<DataInitializer>();

var app = builder.Build();

// Enable swagger UI, viewable at https://localhost:5001/swagger/index.html
app.UseSwagger();
app.UseSwaggerUI();

// Enable authorization for all endpoints below
app.UseAuthentication();
app.UseAuthorization();

// Basic logging middleware that logs all requests to console and debug output
app.Use(async (context, next) =>
{
    await next();

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Log differently based on status code, without a bunch of copy-paste if statements
    Action<string?, object?[]> logFn = context.Response.StatusCode switch
    {
        >= 200 and < 400 => logger.LogInformation,
        _ => logger.LogError,
    };

    logFn.Invoke("{Method} {Path} {StatusCode}",
        new object[]
        {
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode
        });
});

// Map all endpoints in the assembly
app.MapEndpoints<Program>();

// Seed initial data
var dataInitializer = app.Services.GetRequiredService<DataInitializer>();
await dataInitializer.InitializeAsync();

app.Run();
