namespace How.Server.Extensions;

using Common.Configurations;
using Common.Constants;
using Core;
using Core.Database;
using Core.Database.Entities.Identity;
using Core.Infrastructure.Background.BackgroundTaskQueue;
using Core.Infrastructure.Background.Workers;
using Core.Infrastructure.NpgsqlExtensions;
using Core.Services.Identity;
using Core.Services.CurrentUser;
using Core.Services.Storage.FileStorage;
using Core.Services.Storage.ImageStorage;
using Core.Services.Account;
using Core.Services.Event;
using Core.Services.Public.PublicEvent;
using Core.Services.Public.PublicRecord;
using Core.Services.Record;
using Core.Services.SharedUser;
using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hosting.Filters;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;

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

        services.AddControllers(options =>
        {
            options.Filters.Add<ModelStateValidationFilter>();
            options.Filters.Add<ExceptionFilter>();
        }).AddJsonOptions(s => s.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
        
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore
            }
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        
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
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                               throw new ApplicationException("Database Connection string is null!");

        services.AddDbContext<BaseDbContext>(o =>
        {
            o.UseNpgsql(connectionString,
                b =>
                {
                    b.UseNodaTime();
                    b.MigrationsAssembly("How.Server");
                });
            
            o.UseSnakeCaseNamingConvention();
        });

        services.AddDbContextFactory<BaseDbContext>(o => 
            o.UseNpgsql(connectionString), 
            ServiceLifetime.Scoped);

        services.AddSingleton<DapperConnection>(o => new DapperConnection(connectionString));
        
        SqlMapper.AddTypeHandler(InstantHandler.Default);
        
        return services;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminCredentials>(configuration.GetSection("AdminCredentials"));

        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyCoreReference).Assembly);
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
        });
        
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        services.AddTransient<IImageStorageService, ImageStorageService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IRecordService, RecordService>();
        services.AddTransient<ISharedUserService, SharedUserService>();
        
        // Public
        services.AddTransient<IPublicEventService, PublicEventService>();
        services.AddTransient<IPublicRecordService, PublicRecordService>();
        
        // Background
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<QueueHostedService>();
        
        return services;
    }

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(g =>
        {
            g.ConfigureForNodaTime();
            g.EnableAnnotations();
            
            g.SwaggerDoc(SwaggerDocConstants.Account, new OpenApiInfo
            {
                Title = "How",
                Description = "How Account",
                Version = SwaggerDocConstants.Account,
            });
            g.SwaggerDoc(SwaggerDocConstants.Identity, new OpenApiInfo
            {
                Title = "How",
                Description = "How Identity",
                Version = SwaggerDocConstants.Identity,
            });
            g.SwaggerDoc(SwaggerDocConstants.Dashboard, new OpenApiInfo
            {
                Title = "How",
                Description = "How Dashboard",
                Version = SwaggerDocConstants.Dashboard,
            });
            g.SwaggerDoc(SwaggerDocConstants.Public, new OpenApiInfo
            {
                Title = "How",
                Description = "How Public",
                Version = SwaggerDocConstants.Public,
            });
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

    private static IServiceCollection AddCookies(this IServiceCollection services)
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