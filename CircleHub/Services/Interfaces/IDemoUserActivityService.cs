using System.Security.Claims;

namespace CircleHub.Services.Interfaces;

public interface IDemoUserActivityService
{
    Task UpdateActivityAsync(ClaimsPrincipal user);
}
