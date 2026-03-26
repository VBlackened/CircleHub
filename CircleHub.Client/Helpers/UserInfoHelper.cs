using System.Security.Claims;
using CircleHub.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace CircleHub.Client.Helpers
{
    public static class UserInfoHelper
    {
        public static UserInfo? GetUserInfo(AuthenticationState authState)
        {
            ClaimsPrincipal user = authState.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var email = user.FindFirst(ClaimTypes.Email)!.Value;
            var firstName = user.FindFirst("FirstName")!.Value;
            var lastName = user.FindFirst("LastName")!.Value;
            string? profilePictureUrl = user.FindFirst(nameof(UserInfo.ProfilePictureUrl))?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName)
                || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(profilePictureUrl))
            {
                return null;
            }

            UserInfo userInfo = new UserInfo()
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ProfilePictureUrl = profilePictureUrl
            };

            return userInfo;

        }

        public static async Task<UserInfo?> GetUserInfoAsync(Task<AuthenticationState>? authStateTask)
        {
            if (authStateTask == null)
            {
                return null;
            }
            else
            {
                AuthenticationState authState = await authStateTask;
                UserInfo? userInfo = GetUserInfo(authState);
                return userInfo;
            }
        }
    }
}
