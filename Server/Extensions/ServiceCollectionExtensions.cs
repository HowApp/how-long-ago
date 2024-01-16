namespace How.Server.Extensions;

using Core.Database;
using Microsoft.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);
        return services;
    }

    private static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<BaseDbContext>(o =>
        {
            o.UseNpgsql(connectionString,
                b =>
                {
                    b.MigrationsAssembly("How.Server");
                });
            
            o.UseSnakeCaseNamingConvention();
        });

        services.AddDbContextFactory<BaseDbContext>(o => 
            o.UseNpgsql(connectionString), 
            ServiceLifetime.Scoped);
        
        return services;
    }
}