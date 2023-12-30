global using Microsoft.AspNetCore.Components.Authorization;
using Client;
using Client.Utils;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

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
    services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
    services.AddBlazoredLocalStorage();
    services.AddSingleton<GlobalAuth>();
    services.AddSingleton<AuthenticationStateProvider, Authentication>();
    services.AddAuthorizationCore();
    services.AddCascadingValue(sp =>
    {
        bool IsRendered = false;
        var source = new CascadingValueSource<bool>(IsRendered, isFixed: false);
        return source;
    });
}
