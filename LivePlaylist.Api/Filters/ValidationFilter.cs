using FluentValidation;

namespace LivePlaylist.Api.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Attempt to find a validatable object of type T in the arguments
        if (context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) is not T validatable)
        {
            return Results.BadRequest();
        }

        // Attempt to validate the object and return a bad request if it is invalid
        var validationResult = await _validator.ValidateAsync(validatable);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                // Map these to a more user friendly format (exclude a bunch of unnecessary data)
                Errors = validationResult.Errors.Select(e => new { e.ErrorMessage })
            });
        }

        var result = await next.Invoke(context);
        return result;
    }
}
