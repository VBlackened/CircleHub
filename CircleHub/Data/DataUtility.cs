using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CircleHub.Data;

public class DataUtility
{
    public static async Task ManageDataAsync(IServiceProvider svcProvider)
    {
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        var userManagerSvc = svcProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configSvc = svcProvider.GetRequiredService<IConfiguration>();

        await dbContextSvc.Database.MigrateAsync();
        await SeedDemoUserAsync(userManagerSvc, configSvc);
    }

    public static async Task SeedDemoUserAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        try
        {
            string? demoEmail = config["DemoUserLogin"];
            string? demoPassword = config["DemoUserPassword"];

            if (string.IsNullOrEmpty(demoEmail) || string.IsNullOrEmpty(demoPassword))
            {
                throw new Exception("Demo user credentials are not configured properly. Skipping Demo User seeding");
            }

            ApplicationUser? demoUser = new()
            {
                UserName = demoEmail,
                Email = demoEmail,
                FirstName = "Demo",
                LastName = "Login",
                EmailConfirmed = true
            };

            ApplicationUser? user = await userManager.FindByEmailAsync(demoUser.Email);

            if (user == null)
            {
               await userManager.CreateAsync(demoUser, demoPassword);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("************ ERROR *********");
            Console.WriteLine("ERROR Seeding Demo login user.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("****************************");
            throw;
        }
    }
}
