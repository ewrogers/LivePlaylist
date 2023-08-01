# LivePlaylist

This repository serves as a proof of concept for writing a minimal web application that allows users
to create and share playlists of songs.

It is written in .NET Core 7 and follows the C# minimal API conventions, using basic in-memory collection to store data.

Services are injected using the built-in dependency injection container `IServiceCollection`.
This is commonly used in .NET Core applications and allows for easy testing and mocking of services.

## Dependencies

- [.NET Core 7](https://dotnet.microsoft.com/en-us/download)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation) - Model validation
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Swagger documentation

## Project Structure

The project is structured as follows:

- `Auth/` - Contains authorization and authentication schemes
- `Data/` - Contains data parsers and initialization helpers
- `Endpoints/` - Contains the API endpoints for the application
- `Examples/` - Contains examples of accessing the API endpoints
- `Filters/` - Contains the filters (middleware) used by endpoints
- `Models/` - Contains the models used by the application
- `Services/` - Contains the services used by the application
- `Validators/` - Contains the validators used by the application
- `Program.cs` - Entry point for the application that configures the API

## Running the API

To run the API, you can use the following command:

```bash
cd LivePlaylist.Api
dotnet run
```

## Swagger Documentation

The API documentation is automatically generated using Swagger.

You can access the documentation by running the API and navigating to [http://localhost:3000/swagger/index.html](http://localhost:3000/swagger/index.html).

## Authentication

The API uses a very basic "API Key" authentication scheme to simplify the proof of concept:

```
Authorization: User {username}
```

There are no passwords or special JWTs, just a simple username that is used to identify the user making the request.

Obviously this is not secure and would need to be replaced with an actual authentication scheme in a production application.

## Request Flow

The API endpoints generally follow this flow:

```
[Request] -> Auth -> Filters -> Endpoint -> Service -> Data -> [Response]
```

Filters are optional middleware that can be applied to endpoints to perform additional validation or logging.

## Seed Data

The API will automatically seed the in-memory data store with some initial data on startup:

1. A default user named `admin`
2. Over 250 songs from the `songs.csv` file

This can be modified in the `DataInitializer` class in the `InitializeAsync` method.

## Automatic Endpoint Registration

The `EndpointsExtensions` class extension methods that allow automatic service and endpoint registration
for all classes that implement the `IEndpoints` interface within the assembly.

This is called in the `Program.cs` file to register all endpoints without the need to manually register each one.

This helps promote the Open/Closed principle by allowing new endpoints to be added without
having to modify the `Program.cs` file.
