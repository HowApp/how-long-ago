namespace How.Core.Database.Seeds;

using Common.Configurations;
using Common.Constants;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class SeedAdmin
{
    public static async Task Seed(
        IServiceProvider serviceProvider,
        AdminCredentials adminCredentials)
    {
        var userManager = serviceProvider.GetService<UserManager<HowUser>>();
        var user = await userManager!.FindByEmailAsync(adminCredentials.Email);

        if (user is null)
        {
            user = new HowUser
            {
                FirstName = "Admin",
                LastName = string.Empty,
                UserName = adminCredentials.Name,
                Email = adminCredentials.Email,
                EmailConfirmed = true,
                UserRoles = new List<HowUserRole>
                {
                    new ()
                    {
                        RoleId = AppConstants.Role.Admin.Id
                    }
                }
            };

            var result = await userManager!.CreateAsync(user, adminCredentials.Password);
            
            if (!result.Succeeded)
            {
                throw new Exception("Admin not created");
            }
            
            await userManager.UpdateAsync(user);
        }
    }
}