namespace How.Core.Database.Seeds;

using Common.Configurations;
using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SeedAdmin
{
    public static async Task Seed(
        IServiceProvider serviceProvider,
        AdminCredentials adminCredentials)
    {
        var context = serviceProvider.GetRequiredService<BaseDbContext>();
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == adminCredentials.UserId);

        if (user is null)
        {
            user = new HowUser
            {
                UserId = adminCredentials.UserId,
                FirstName = adminCredentials.FirstName,
                LastName = adminCredentials.LastName
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}