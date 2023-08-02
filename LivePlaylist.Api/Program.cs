using System.Diagnostics;
using FluentValidation;
using LivePlaylist.Api.Auth;
using LivePlaylist.Api.Data;
using LivePlaylist.Api.Endpoints;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    // Allow requests from localhost:3000 (Swagger) and localhost:3001 (UI)
    options.AddDefaultPolicy(x => x.WithOrigins("localhost:3000", "localhost:3001")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

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
});

builder.Services.AddSingleton<DataInitializer>();

var app = builder.Build();

app.UseCors();

// Enable swagger UI, viewable at /swagger/index.html
app.UseSwagger();
app.UseSwaggerUI();

// Basic logging middleware that logs all requests to console and debug output
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Log differently based on status code, without a bunch of copy-paste if statements
    Action<string?, object?[]> logFn = context.Response.StatusCode switch
    {
        >= 200 and < 400 => logger.LogInformation,
        _ => logger.LogError,
    };

    logFn.Invoke("{Method} {Path} {StatusCode} - {Elapsed}ms",
        new object[]
        {
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds)
        });
});

// Map all endpoints in the assembly
app.MapEndpoints<Program>();

// Seed initial data
var dataInitializer = app.Services.GetRequiredService<DataInitializer>();
await dataInitializer.InitializeAsync();

app.Run();
