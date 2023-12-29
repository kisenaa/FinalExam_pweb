global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
using Client;
using Client.Utils;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (!builder.RootComponents.Any())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection services, string baseAddress)
{
    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
    services.AddBlazoredLocalStorage();
    services.AddScoped<AuthenticationStateProvider, Authentication>();
    services.AddScoped<Authentication>();
    services.AddAuthorizationCore();
}

