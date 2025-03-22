using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Autobody.Data
{
    public static class RepositoryConfiguration
    {
        public static IServiceCollection AddAutobodyRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDateTimeProvider, AutobodyDateTimeProvider>();

            services
                .AddScoped<IUserContext<User, Log>, AutobodyContext>()
                .AddScoped<IRoleContext<Role, Log>, AutobodyContext>()
                .AddScoped<IGroupContext<Group, Log>, AutobodyContext>()
                .AddScoped<IUser_GroupContext<User_Group, Log>, AutobodyContext>()
                .AddScoped<IRole_GroupContext<Role_Group, Log>, AutobodyContext>()

                .AddScoped<ILogContext<Log>, AutobodyContext>();

            services
                // Identity
                .AddScoped<UserRepository>()
                .AddScoped<RoleRepository>()
                .AddScoped<GroupRepository>()
                .AddScoped<User_GroupRepository>()
                .AddScoped<Role_GroupRepository>();

            return services;
        }
    }
}
