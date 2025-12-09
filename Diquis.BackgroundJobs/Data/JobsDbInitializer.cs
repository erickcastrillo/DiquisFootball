using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diquis.BackgroundJobs.Areas.Identity.Data;
using System.Security.Claims;

namespace Diquis.BackgroundJobs.Data;

public static class JobsDbInitializer
{
    public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            UserManager<DiquisBackgroundJobsUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<DiquisBackgroundJobsUser>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            DiquisInfrastructureJobsContext context = scope.ServiceProvider.GetRequiredService<DiquisInfrastructureJobsContext>();

            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roleNames = { "root", "admin", "user" }; // Example roles, adjust as needed
            foreach (string roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    _ = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Root User
            string rootEmail = "admin@job.com"; // Separate admin for jobs dashboard
            string rootPassword = "Password123!"; // Strong password
            string rootRole = "root";

            DiquisBackgroundJobsUser? rootUser = await userManager.FindByEmailAsync(rootEmail);
            if (rootUser == null)
            {
                rootUser = new DiquisBackgroundJobsUser
                {
                    UserName = rootEmail,
                    Email = rootEmail,
                    EmailConfirmed = true,
                };
                IdentityResult result = await userManager.CreateAsync(rootUser, rootPassword);
                if (result.Succeeded)
                {
                    _ = await userManager.AddToRoleAsync(rootUser, rootRole);
                }
            }
        }
    }
}
