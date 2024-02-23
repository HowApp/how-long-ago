
namespace How.Server;

using Common.Configurations;
using Common.Constants;
using Core.Database;
using Core.Database.Seeds;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.

        builder.Services.SetupServices(builder.Configuration);
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        
        var app = builder.Build();

        var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
        using (var scope = serviceScopeFactory!.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
            await dbContext!.Database.MigrateAsync();
            
            var adminCredentials = scope.ServiceProvider.GetService<IOptions<AdminCredentials>>();
            await SeedAdmin.Seed(scope.ServiceProvider, adminCredentials!.Value);
        }
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseSwagger();
            app.UseSwaggerUI();
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

        app.MapRazorPages();
        app.MapControllers();
        
        app.UseStaticFiles();
        
        // app.UseBlazorFrameworkFiles();
        // app.MapFallbackToFile("index.html");

        await app.RunAsync();
    }
}
