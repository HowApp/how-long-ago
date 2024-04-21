namespace How.Server.Extensions;

using Common.Configurations;
using Common.Constants;
using Core;
using Core.Database;
using Core.Database.Entities.Identity;
using Core.Services.AccountServices;
using Core.Services.UserServices;
using Core.Services.Storage;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hosting.Filters;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseAppSettings = new BaseApplicationSettings();
        configuration.Bind(nameof(BaseApplicationSettings), baseAppSettings);
        
        services.AddCors(options =>
        {
            options.AddPolicy(AppConstants.CorsPolicy, builder =>
            {
                builder.WithOrigins(baseAppSettings.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ModelStateValidationFilter>();
            options.Filters.Add<ExceptionFilter>();
        });
        
        services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<BaseDbContext>();
        
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        services.AddDataAccess(configuration)
            .AddConfigurations(configuration)
            .AddIdentity()
            .AddCustomServices()
            .AddSwagger()
            .AddCookies();
        
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

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminCredentials>(configuration.GetSection("AdminCredentials"));

        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyCoreReference).Assembly);
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
        });
        
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IStorageService, StorageService>();
        
        return services;
    }

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<HowUser, HowRole>()
            .AddEntityFrameworkStores<BaseDbContext>();

        services.Configure<IdentityOptions>(o =>
        {
            // Password settings.
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireNonAlphanumeric = true;
            o.Password.RequireUppercase = true;
            o.Password.RequiredLength = 8;
            o.Password.RequiredUniqueChars = 4;
            
            // Lockout settings.
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.AllowedForNewUsers = true;
            
            // User settings.
            o.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            o.User.RequireUniqueEmail = true;
        });

        return services;
    }

    public static IServiceCollection AddCookies(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(o =>
        {
            // Cookie settings
            o.Cookie.HttpOnly = false;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(60);

            o.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        return services;
    }
}