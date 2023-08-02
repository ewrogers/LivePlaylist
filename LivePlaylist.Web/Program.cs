using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LivePlaylist.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
    new HttpClient
    {
        DefaultRequestHeaders =
        {
            Authorization = new AuthenticationHeaderValue("User", "admin")
        },
        BaseAddress = new Uri("http://localhost:3000")
    });

await builder.Build().RunAsync();
