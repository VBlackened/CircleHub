using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CircleHub.Client.Helpers
{
    public static class UserInfoHelper
    {
        public static async Task<UserInfo> GetUserInfoAsync(Task<AuthenticationState>? authStateTask)
        {
            if (authStateTask is null) return null;

            AuthenticationState authState = await authStateTask;
            ClaimsPrincipal user = authState.User;

            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var email = user.FindFirst(ClaimTypes.Email)!.Value;
                var fisrtName = user.FindFirst("FirstName")!.Value;
                var lastName = user.FindFirst("LastName")!.Value;

                return new UserInfo
                {
                    UserId = userId,
                    Email = email,
                    FirstName = fisrtName,
                    LastName = lastName
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
