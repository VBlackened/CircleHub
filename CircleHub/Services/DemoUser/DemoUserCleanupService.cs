using CircleHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CircleHub.Services.DemoUser;

public class DemoUserCleanupService(IServiceScopeFactory scopeFactory, ILogger<DemoUserCleanupService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<DemoUserCleanupService> _logger = logger;

    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DemoLifetime = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(CleanupInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await CleanupDemoUsersAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while cleaning up demo users.");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
    }

    private async Task CleanupDemoUsersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var expirationTime = DateTime.UtcNow.Subtract(DemoLifetime);

        var demoUsers = await context.Users
            .Where(u => u.IsDemo && u.DemoLastActivity < expirationTime)
            .Include(u => u.Contacts)
            .ToListAsync(cancellationToken);

        foreach (var user in demoUsers)
        {
            _logger.LogInformation(
                "Removing expired demo user {UserId}.",
                user.Id);

            var imageIds = user.Contacts
                .Where(c => c.ImageId.HasValue)
                .Select(c => c.ImageId!.Value)
                .ToList();

            if (user.ProfilePictureId.HasValue)
            {
                imageIds.Add(user.ProfilePictureId.Value);
            }

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                _logger.LogError(
                    "Failed to delete demo user {UserId}: {Errors}",
                    user.Id,
                    errors);

                continue;
            }

            if (imageIds.Count > 0)
            {
                await context.Images
                    .Where(i => imageIds.Contains(i.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }
    }
}