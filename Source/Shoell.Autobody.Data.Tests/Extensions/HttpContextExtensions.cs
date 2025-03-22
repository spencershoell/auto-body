using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Extensions;
using ClaimTypes = Shoell.Shared.Types.ClaimTypes;

namespace Shoell.Autobody.Data.Tests
{
    public static class HttpContextExtensions
    {
        public static HttpContext ResetUser(this HttpContext httpContext)
        {
            httpContext.User = new ClaimsPrincipal();
            return httpContext;
        }

        public async static Task<HttpContext> SetUserWithRolesAsync(this HttpContext httpContext, Guid userId, AutobodyContext context)
        {
            var principal = new ClaimsPrincipal();

            var user = await context.Users
                .Include(e => e.UserRoles)
                    .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (user != null)
            {
                var claimsIdentity = new ClaimsIdentity("Test");
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyIdClaimType, user.Id.ToString()));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyExternalIdClaimType, user.ExternalId));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyNameClaimType, user.Name));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyUserNameClaimType, user.UserName ?? string.Empty));

                foreach (var userRole in user.UserRoles)
                    if (!principal.HasClaim(claim => claim.Type == ClaimTypes.AutobodyRole && claim.Value == userRole.Role.Name))
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.AutobodyRole, userRole.Role.Name ?? string.Empty));
                principal.AddIdentity(claimsIdentity);
            }

            httpContext.User = principal;

            return httpContext;
        }
    }
}
