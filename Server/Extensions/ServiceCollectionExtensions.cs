namespace How.Server.Extensions;

using Common.Configurations;
using Common.Constants;
using Core;
using Core.Database;
using Core.Infrastructure.Background.BackgroundTaskQueue;
using Core.Infrastructure.Background.Workers;
using Core.Infrastructure.NpgsqlExtensions;
using Core.Infrastructure.Processing.Consumer;
using Core.Services.Identity;
using Core.Services.CurrentUser;
using Core.Services.Storage.FileStorage;
using Core.Services.Storage.ImageStorage;
using Core.Services.Account;
using Core.Services.BackgroundImageProcessing;
using Core.Services.Event;
using Core.Services.GrpcCommunication;
using Core.Services.Hubs.FileProcessingHubService;
using Core.Services.Public.PublicEvent;
using Core.Services.Public.PublicRecord;
using Core.Services.Record;
using Core.Services.SharedUser;
using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hosting;
using Hosting.Filters;
using HowCommon.Configurations;
using MassTransit;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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

        services.AddHttpContextAccessor();

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
            .AddCustomServices()
            .AddGrpcServices()
            .ConfigureMassTransit(configuration)
            .AddCustomAuthentication(configuration)
            .AddSwagger()
            .AddSignalR();
        
        return services;
    }

    private static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMq = new RabbitMqConfiguration();
        configuration.Bind(nameof(RabbitMqConfiguration), rabbitMq);
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisterConsumer, UserRegisterConsumerDefinition>();
            x.AddConsumer<UserDeletedConsumer, UserDeletedConsumerDefinition>();
            x.AddConsumer<UserSuspendStateConsumer, UserSuspendStateConsumerDefinition>();
            x.AddConsumer<UserRegisterBulkConsumer, UserRegisterBulkConsumerDefinition>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("dev", false));

            x.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitMq.Host, "/", host =>
                {
                    host.Username(rabbitMq.User);
                    host.Password(rabbitMq.Password);
                });

                config.ConfigureEndpoints(context);
            });
        });

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(15);
                options.StopTimeout = TimeSpan.FromSeconds(30);
            });
        
        services.AddOptions<HostOptions>()
            .Configure(options =>
            {
                options.StartupTimeout = TimeSpan.FromSeconds(30);
                options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            });
        
        return services;
    }
    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identityServerConfiguration = new IdentityServerConfiguration();
        configuration.Bind(nameof(IdentityServerConfiguration), identityServerConfiguration);
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // base-address of identity server
                options.Authority = identityServerConfiguration.Authority;
                options.Audience = identityServerConfiguration.Audience;
        
                options.TokenValidationParameters.ValidateAudience = true;
        
                // it's recommended to check the type header to avoid "JWT confusion" attacks
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                options.MapInboundClaims = false;
        
                // if token does not contain a dot, it is a reference token
                options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
            })
            .AddOAuth2Introspection("introspection", options =>
            {
                options.Authority = identityServerConfiguration.Authority;
        
                options.ClientId = identityServerConfiguration.ClientId;
                options.ClientSecret = identityServerConfiguration.ClientSecret;
            });
        
        return services;
    }
    private static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                               throw new ApplicationException("Database Connection string is null!");
        
        var connectionTemporaryString = configuration.GetConnectionString("TemporaryStorageConnection") ?? 
                               throw new ApplicationException("Database Temporary Connection string is null!");

        services.AddDbContext<BaseDbContext>(o =>
        {
            o.UseNpgsql(connectionString,
                b =>
                {
                    b.UseNodaTime();
                    b.MigrationsAssembly("How.Server");

                    b.MigrationsHistoryTable(tableName: HistoryRepository.DefaultTableName, schema: "public");
                });
            
            o.UseSnakeCaseNamingConvention();
        });

        services.AddDbContext<TemporaryStorageDbContext>(o =>
        {
            o.UseNpgsql(connectionTemporaryString,
                b =>
                {
                    b.MigrationsAssembly("How.Server");
                    
                    b.MigrationsHistoryTable(tableName: HistoryRepository.DefaultTableName, schema: "temporary");
                });
            
            o.UseSnakeCaseNamingConvention();
        });

        services.AddDbContextFactory<BaseDbContext>(o => 
            o.UseNpgsql(connectionString), 
            ServiceLifetime.Scoped);

        services.AddDbContextFactory<TemporaryStorageDbContext>(o => 
                o.UseNpgsql(connectionTemporaryString), 
            ServiceLifetime.Scoped);

        services.AddSingleton<DapperConnection>(o => 
            new DapperConnection(connectionString, connectionTemporaryString));
        
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
        services.AddTransient<IBackgroundImageProcessing, BackgroundImageProcessing>();
        
        // Hub
        services.AddTransient<IFileProcessingHubService, FileProcessingHubService>();
        
        return services;
    }

    private static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddTransient<UserAccountGrpcService>();
        
        return services;
    }

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

            g.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    Name = "How API Swagger Client",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5001/connect/token"),
                            Scopes = new Dictionary<string, string> 
                            {
                                {"scope.how-api", "How API"},
                            }
                        }
                    }   
                });

            g.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }
}