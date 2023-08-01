using FluentValidation;
using LivePlaylist.Api.Auth;
using LivePlaylist.Api.Data;
using LivePlaylist.Api.Endpoints;
using Microsoft.AspNetCore.Authorization;
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

// Add authorization policy (requires authentication for all endpoints behind auth)
builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;
    
    // Ensures that 401 errors are generated for undefined endpoints (instead of 404)
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Enable OpenAPI/Swagger document generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add security definition for API key to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid API key",
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "User: {username}",
        Scheme = "Bearer"
    });

    // Add security requirement for API key to all routes in Swagger UI
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference()
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

// Enable swagger UI, viewable at https://localhost:5001/swagger/index.html
app.UseSwagger();
app.UseSwaggerUI();

// Enable authorization for all endpoints below (intentionally after Swagger UI)
app.UseAuthentication();
app.UseAuthorization();

// Map all endpoints in the assembly
app.UseEndpoints<Program>();

// Seed initial data
var dataInitializer = app.Services.GetRequiredService<DataInitializer>();
await dataInitializer.InitializeAsync();

app.Run();
