using Microsoft.AspNetCore.ResponseCompression;

namespace How.Server;

using Core.Database;
using Extensions;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.SetupServices(builder.Configuration);
        
        var app = builder.Build();

        var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
        using (var scope = serviceScopeFactory!.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
            await dbContext!.Database.MigrateAsync();
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

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        await app.RunAsync();
    }
}
