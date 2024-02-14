namespace How.Server.Extensions;

using Core.Database;
using Core.Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration)
            .AddSwaggerGen();
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

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<HowUser, IdentityRole>()
            .AddEntityFrameworkStores<BaseDbContext>();

        services.Configure<IdentityOptions>(o =>
        {
            // Password settings.
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireNonAlphanumeric = true;
            o.Password.RequireUppercase = true;
            o.Password.RequiredLength = 12;
            o.Password.RequiredUniqueChars = 4;
            
            // Lockout settings.
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.AllowedForNewUsers = true;
            
            // User settings.
            o.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            o.User.RequireUniqueEmail = false;
        });

        return services;
    }

    public static IServiceCollection AddCookies(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(o =>
        {
            // Cookie settings
            o.Cookie.HttpOnly = true;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(30);

            o.LoginPath = "/Identity/Account/Login";
            o.AccessDeniedPath = "Identity/Account/AccessDenied";
            o.SlidingExpiration = true;
        });

        return services;
    }
}