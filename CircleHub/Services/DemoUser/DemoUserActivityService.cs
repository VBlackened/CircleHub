using CircleHub.Data;
using CircleHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace CircleHub.Services.DemoUser;

public class DemoUserActivityService(IDbContextFactory<ApplicationDbContext> contextFactory) : IDemoUserActivityService
{
    public async Task UpdateActivityAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return;

        await using var context = await contextFactory.CreateDbContextAsync();

        var demoUser = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsDemo);

        if (demoUser is null)
            return;

        demoUser.DemoLastActivity = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }
}