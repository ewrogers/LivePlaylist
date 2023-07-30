namespace LivePlaylist.Api.Endpoints;

public interface IEndpoints
{
    public static abstract void AddServices(IServiceCollection services, IConfiguration configuration);
    
    public static abstract void MapEndpoints(IEndpointRouteBuilder app);
}
