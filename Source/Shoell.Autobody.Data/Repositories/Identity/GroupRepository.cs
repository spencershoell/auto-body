using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public class GroupRepository(
        IAuthorizationService authorizationService,
        IGroupContext<Group, Log> context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider
    ) : BaseRepository<Group>
    {
        protected override EntityType EntityType => Group.EntityType;

        protected override IAuthorizationService AuthorizationService => authorizationService;
        protected override IGroupContext<Group, Log> Context => context;
        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected IServiceProvider ServiceProvider => serviceProvider;
        protected override DbSet<Group> EntitySet => Context.Groups;

        protected override string CreatePolicy => GroupRoles.Create;
        protected override string ReadPolicy => GroupRoles.Read;
        protected override string UpdatePolicy => GroupRoles.Update;
        protected override string DeletePolicy => GroupRoles.Delete;

        protected override string RecyclePolicy => GroupRoles.Recycle;
        protected override string RecoverPolicy => GroupRoles.Recover;
        protected override string PurgePolicy => GroupRoles.Purge;

        protected override string ArchivePolicy => GroupRoles.Archive;
        protected override string RestorePolicy => GroupRoles.Restore;

        protected override Task<bool> ArchiveChildrenAsync(Group model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> PurgeChildrenAsync(Group model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RestoreChildrenAsync(Group model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RemoveChildrenAsync(Group model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RecoverChildrenAsync(Group model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override void FilterUpdateProperties(Delta<Group> delta, Group model)
        {
        }
    }
}
