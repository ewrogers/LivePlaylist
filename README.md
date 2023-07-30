# LivePlaylist

This repository serves as a proof of concept for writing a minimal web application that allows users to create and share playlists of songs.

It is written in .NET Core 7 and follows the minimal API conventions.

## Dependencies

- [.NET Core 7](https://dotnet.microsoft.com/en-us/download)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## Project Structure

The project is structured as follows:

- `Endpoints/` - Contains the API endpoints for the application
- `Models/` - Contains the models used by the application
- `Services/` - Contains the services used by the application
- `Validators/` - Contains the validators used by the application
- `Program.cs` - Entry point for the application that configures the API

## API Registration

The `EndpointsExtensions` class extension methods that allow automatic service and endpoint registration
for all classes that implement the `IEndpoints` interface.

This is called in the `Program.cs` file to register all endpoints without the need to manually register each one.

This helps promote the Open/Closed principle by allowing new endpoints to be added without having to modify the `Program.cs` file.

## Running the API

To run the API, you can use the following command:

```bash
cd LivePlaylist.Api
dotnet run
```

## Swagger Documentation

The API documentation is automatically generated using Swagger.

You can access the documentation by running the API and navigating to [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html).
