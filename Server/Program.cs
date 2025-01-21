namespace How.Server;

using Common.Configurations;
using Common.Constants;
using Core.Database;
using Core.Database.Seeds;
using Core.Infrastructure.Hubs;
using Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(7060, listenOptions =>
        {
            listenOptions.UseHttps();
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        }));

        // Add services to the container.
        builder.Services.SetupServices(builder.Configuration);

        var app = builder.Build();

        var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
        using (var scope = serviceScopeFactory!.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
            await dbContext!.Database.MigrateAsync();

            var temporaryDbContext = scope.ServiceProvider.GetRequiredService<TemporaryStorageDbContext>();
            await temporaryDbContext!.Database.MigrateAsync();
            await temporaryDbContext!.Database.ExecuteSqlAsync(sql: $"TRUNCATE TABLE temporary.files RESTART IDENTITY;");

            var adminCredentials = scope.ServiceProvider.GetService<IOptions<AdminCredentials>>();
            await SeedAdmin.Seed(scope.ServiceProvider, adminCredentials!.Value);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/swagger/{SwaggerDocConstants.Account}/swagger.json", SwaggerDocConstants.Account);
                s.SwaggerEndpoint($"/swagger/{SwaggerDocConstants.Identity}/swagger.json", SwaggerDocConstants.Identity);
                s.SwaggerEndpoint($"/swagger/{SwaggerDocConstants.Dashboard}/swagger.json", SwaggerDocConstants.Dashboard);
                s.SwaggerEndpoint($"/swagger/{SwaggerDocConstants.Public}/swagger.json", SwaggerDocConstants.Public);

                s.OAuthClientId("how-api-swagger-client");
                // s.OAuthScopes(new[]{"scope.how-api"}); //add scope by default every login request
                s.OAuthClientSecret(app.Configuration.GetSection("IdentityServerConfiguration:ClientSwaggerSecret").Value);
                s.OAuthUsePkce();
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseCors(AppConstants.CorsPolicy);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles();
        
        app.MapHub<FileProcessingHub>("/hubs/fileProcessing");

        await app.RunAsync();
    }
}
