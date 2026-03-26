using System.Security.Claims;
using CircleHub.Client.Models;
using CircleHub.Data;
using CircleHub.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CircleHub.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> options)
                                                                : UserClaimsPrincipalFactory<ApplicationUser>(userManager, options)
    {
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            string? profilePictureUrl = user.ProfilePictureId.HasValue
                ? $"/uploads/{user.ProfilePictureId.Value}"
                : ImageHelper.DefaultProfilePictureUrl;

            List<Claim> customClaims =
            [
                new Claim(nameof(UserInfo.ProfilePictureUrl), profilePictureUrl),
                new Claim("FirstName", user.FirstName!),
                new Claim("LastName", user.LastName!),
            ];

            identity.AddClaims(customClaims);

            return identity;
        }
    }
}
