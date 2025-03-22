using Microsoft.AspNetCore.Authentication;
using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Services
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection AddAutobodyAuthorization(this IServiceCollection services)
        {
            services.AddScoped<IUserContext<User, Log>, AutobodyContext>();
            services.AddTransient<IClaimsTransformation, AutobodyClaimsTransformation>();

            services.AddAuthorizationBuilder()
                // Group          
                .AddPolicy(GroupRoles.Create, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Create]))
                .AddPolicy(GroupRoles.Read, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Read, GroupRoles.Create, GroupRoles.Update, GroupRoles.Delete, GroupRoles.Archive, GroupRoles.Restore]))
                .AddPolicy(GroupRoles.Update, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Update]))
                .AddPolicy(GroupRoles.Delete, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Delete]))
                .AddPolicy(GroupRoles.Recycle, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Recycle, GroupRoles.Recover, GroupRoles.Purge]))
                .AddPolicy(GroupRoles.Recover, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Recover]))
                .AddPolicy(GroupRoles.Purge, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Purge]))
                .AddPolicy(GroupRoles.Archive, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Archive]))
                .AddPolicy(GroupRoles.Restore, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Restore]))

                // Role
                .AddPolicy(RoleRoles.Read, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [RoleRoles.Read]))

                // User
                .AddPolicy(UserRoles.Create, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Create]))
                .AddPolicy(UserRoles.Read, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Read, UserRoles.Create, UserRoles.Update, UserRoles.Delete, UserRoles.Archive, UserRoles.Restore]))
                .AddPolicy(UserRoles.Update, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Update]))
                .AddPolicy(UserRoles.Delete, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Delete]))
                .AddPolicy(UserRoles.Recycle, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Recycle, UserRoles.Recover, UserRoles.Purge]))
                .AddPolicy(UserRoles.Recover, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Recover]))
                .AddPolicy(UserRoles.Purge, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Purge]))
                .AddPolicy(UserRoles.Archive, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Archive, UserRoles.Restore]))
                .AddPolicy(UserRoles.Restore, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Restore]))

                // Role_Group
                .AddPolicy(Role_GroupRoles.Read, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Read, RoleRoles.Read]))
                .AddPolicy(Role_GroupRoles.Modify, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [GroupRoles.Update]))

                // User_Group
                .AddPolicy(User_GroupRoles.Read, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Read, GroupRoles.Read]))
                .AddPolicy(User_GroupRoles.Modify, policy => policy.RequireClaim(ClaimTypes.AutobodyRole, [UserRoles.Update, GroupRoles.Update]));

            return services;
        }
    }
}
