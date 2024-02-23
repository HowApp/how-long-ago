using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace How.Client;

using Extensions;

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
        
        var url = builder.Configuration.GetValue<string>("AppConfigurations:ApiUrl");
        builder.Services.AddScoped(sp => 
            new HttpClient
            {
                BaseAddress = new Uri(url?? builder.HostEnvironment.BaseAddress)
            });

        await builder.Build().RunAsync();
    }
}
