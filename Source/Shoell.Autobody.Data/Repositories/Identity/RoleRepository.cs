using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public class RoleRepository(
        IAuthorizationService authorizationService,
        IRoleContext<Role, Log> context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider
    ) : CoreRepository<Role>
    {
        protected override EntityType EntityType => Role.EntityType;

        protected override IAuthorizationService AuthorizationService => authorizationService;
        protected override IRoleContext<Role, Log> Context => context;
        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected IServiceProvider ServiceProvider => serviceProvider;
        protected override DbSet<Role> EntitySet => Context.Roles;

        protected override string ReadPolicy => RoleRoles.Read;

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Get()
                .AnyAsync(e =>
                       e.Id.Equals(id), cancellationToken)
                || EntitySet.Local.Any(e => e.Id.Equals(id));
        }
    }
}
