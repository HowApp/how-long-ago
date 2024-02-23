namespace How.Client.Extensions;

using Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Services.Auth;
using Services.Provider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurations(configuration)
            .AddCustomServices();

        return services;
    }
    
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<CustomStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomStateProvider>());
        services.AddScoped<IAuthServices, AuthServices>();
        
        return services;
    }
    
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfigurations = new AppConfigurations();
        configuration.Bind(appConfigurations);
        services.AddSingleton(appConfigurations);
        
        return services;
    }
    
    
}