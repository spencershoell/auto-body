using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces.Identity;

namespace Shoell.Autobody.Identity
{
    public class AutobodyClaimsTransformation(IUserContext<User, Log> userContext) : IClaimsTransformation
    {
        protected IUserContext<User, Log> UserContext => userContext;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var user = await UserContext.Users
                .Include(e => e.UserRoles)
                    .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(e => e.ExternalId == principal.OpenConnectId().ToString());

            if (user != null)
            {
                var claimsIdentity = new ClaimsIdentity("Autobody");
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyIdClaimType, user.Id.ToString()));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyExternalIdClaimType, user.ExternalId));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyNameClaimType, user.Name));
                claimsIdentity.AddClaim(new Claim(ClaimsPrincipalExtensions.AutobodyUserNameClaimType, user.UserName ?? string.Empty));
                foreach (var role in user.UserRoles)
                    if (!principal.HasClaim(claim => claim.Type == Shared.Types.ClaimTypes.AutobodyRole && claim.Value == role.Role.Name))
                        claimsIdentity.AddClaim(new(Shared.Types.ClaimTypes.AutobodyRole, role.Role.Name ?? string.Empty));
                principal.AddIdentity(claimsIdentity);
            }

            return principal;
        }
    }
}
