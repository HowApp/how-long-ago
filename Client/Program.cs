namespace How.Client;

using Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Services.ClientAPI;
using Services.CookieHandler;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        
        builder.Services.SetupServices(builder.Configuration);
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        
        builder.Services.AddScoped(sp => 
            new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration.GetValue<string>("AppConfigurations:FrontendUrl") ?? 
                                      builder.HostEnvironment.BaseAddress)
            });

        builder.Services.AddHttpClient<AuthorizedClientAPI>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AppConfigurations:BackendUrl") ??
                                         builder.HostEnvironment.BaseAddress);
        }).AddHttpMessageHandler<CookieHandler>();
        
        builder.Services.AddHttpClient<AnonymousClientAPI>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AppConfigurations:BackendUrl") ??
                                         builder.HostEnvironment.BaseAddress);
        });
        
        await builder.Build().RunAsync();
    }
}
