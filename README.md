# LivePlaylist

This repository serves as a proof of concept for writing a minimal web application that allows users
to create and share playlists of songs.

The application is split into three projects:

- `LivePlaylist.Api` - Minimal API service that allows users to create and modify playlists
- `LivePlaylist.Web` - Blazor Web Assembly app that acts as a front-end for the API
- `LivePlaylist.Tests` - Unit tests for the API services

## Dependencies

- [.NET Core 7](https://dotnet.microsoft.com/en-us/download)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation) - Model validation
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Swagger documentation
- [TailwindCSS](https://tailwindcss.com/) - CSS framework & design system

## API Project Structure

The API project is structured as follows:

- `Auth/` - Contains authorization and authentication schemes
- `Data/` - Contains data parsers and initialization helpers
- `Endpoints/` - Contains the API endpoints for the application
- `Examples/` - Contains examples of accessing the API endpoints
- `Filters/` - Contains the filters (middleware) used by endpoints
- `Models/` - Contains the models used by the application
- `Services/` - Contains the services used by the application
- `Validators/` - Contains the validators used by the application
- `Program.cs` - Entry point for the application that configures the API

Services are injected using the built-in dependency injection container `IServiceCollection`.
This is commonly used in .NET Core applications and allows for easy testing and mocking of services.

### Running the API

To run the API, you can use the following command:

```bash
cd LivePlaylist.Api
dotnet run
```

This will start the API on [http://localhost:3000](http://localhost:3000).

### Swagger Documentation

The API documentation is automatically generated using Swagger.

You can access the documentation by running the API and navigating to [http://localhost:3000/swagger/index.html](http://localhost:3000/swagger/index.html).

### Unit Tests

To run the unit tests, you can use the following command:

```bash
dotnet test -l "console;verbosity=normal"
```

### Authentication

The API uses a very basic "API Key" authentication scheme to simplify the proof of concept:

```
Authorization: User {username}
```

There are no passwords or special JWTs, just a simple username that is used to identify the user making the request.

Obviously this is not secure and would need to be replaced with an actual authentication scheme in a production application.

### Request Flow

The API endpoints generally follow this flow:

```
[Request] -> Auth -> Filters -> Endpoint -> Service -> Data -> [Response]
```

Filters are optional middleware that can be applied to endpoints to perform additional validation or logging.

### Seed Data

The API will automatically seed the in-memory data store with some initial data on startup:

1. A default user named `admin`
2. Over 250 songs from the `songs.csv` file

This can be modified in the `DataInitializer` class in the `InitializeAsync` method.

### Automatic Endpoint Registration

The `EndpointsExtensions` class extension methods that allow automatic service and endpoint registration
for all classes that implement the `IEndpoints` interface within the assembly.

This is called in the `Program.cs` file to register all endpoints without the need to manually register each one.

This helps promote the Open/Closed principle by allowing new endpoints to be added without
having to modify the `Program.cs` file.

## Web UI Project Structure

The web UI is a Blazor Web Assembly project and structured as follows:

- `wwwroot/` - Contains static files for HTML, CSS, and JavaScript
- `Pages/` - Contains the Blazor pages
- `Shared/` - Contains shared Blazor components
- `Styles/` - Contains the TailwindCSS style definitions
- `MainLayout.razor` - The main layout used by all pages
- `Program.cs` - Entry point for the application that configures the Blazor app

### Running the Web UI

To run the web UI, you can use the following command:

```bash
cd LivePlaylist.Web
dotnet run
```
This will start the web UI on [http://localhost:3001](http://localhost:3000).

### Hot-reloading

As of .NET 6, hot-reloading is enabled by default for Blazor Web Assembly projects:

```bash
cd LivePlaylist.Web
dotnet watch
```

This will automatically recompile and reload the web UI when changes are made.

### TailwindCSS Generation

The TailwindCSS styles are generated using the `TailwindCSS` CLI tool:

```bash
cd LivePlaylist.Web
npx tailwindcss -i ./Styles/app.css -o ./wwwroot/css/app.css --watch
```

This will automatically recompile the TailwindCSS styles when changes are made, and works well with hot-reloading.
