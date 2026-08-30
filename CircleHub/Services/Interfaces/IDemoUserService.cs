using CircleHub.Data;

namespace CircleHub.Services.Interfaces;

public interface IDemoUserService
{
    Task<ApplicationUser> CreateDemoUserAsync();
}
