namespace LivePlaylist.Api.Endpoints;

public static class EndpointsExtensions
{
    public static void AddEndpoints<T>(this IServiceCollection services, IConfiguration configuration)
        => AddEndpoints(services, typeof(T), configuration);
    
    public static void AddEndpoints(this IServiceCollection services, Type type, IConfiguration configuration)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(type);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.AddServices))!
                .Invoke(null, new object[] { services, configuration });
        }
    }

    public static void MapEndpoints<T>(this IApplicationBuilder app)
        => MapEndpoints(app, typeof(T));

    public static void MapEndpoints(this IApplicationBuilder app, Type type)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(type);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.MapEndpoints))!
                .Invoke(null, new object[] { app });
        }
    }

    private static IEnumerable<Type> GetEndpointTypesFromAssemblyContaining(Type type)
    {
        return type.Assembly.DefinedTypes
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpoints).IsAssignableFrom(t));
    }
}
