using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Exceptions;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public class Role_GroupRepository(
        IAuthorizationService authorizationService,
        IRole_GroupContext<Role_Group, Log> context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider
        ) : BaseJoinRepository<Role_Group>
    {
        protected override EntityType EntityType => Role_Group.EntityType;
        protected override EntityType EntityType1 => Role.EntityType;
        protected override EntityType EntityType2 => Group.EntityType;

        protected override IAuthorizationService AuthorizationService => authorizationService;
        protected override IRole_GroupContext<Role_Group, Log> Context => context;
        protected override DbSet<Role_Group> EntitySet => Context.RoleGroups;
        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected IServiceProvider ServiceProvider => serviceProvider;

        protected virtual Lazy<GroupRepository> GroupRepository => new(ServiceProvider.GetRequiredService<GroupRepository>());
        protected virtual Lazy<RoleRepository> RoleRepository => new(ServiceProvider.GetRequiredService<RoleRepository>());

        protected override string ModifyPolicy => Role_GroupRoles.Modify;
        protected override string ReadPolicy => Role_GroupRoles.Read;

        protected override Expression<Func<Role_Group, bool>> IdentifierPredicate(Guid roleId, Guid groupId)
        {
            return e => e.RoleId == roleId && e.GroupId == groupId;
        }

        public override async Task<bool> ExistsAsync(Role_Group model, CancellationToken cancellationToken = default)
        {
            return await ExistsAsync(model.RoleId, model.GroupId, cancellationToken);
        }

        public async Task AddToRoles(Guid groupId, List<Guid> roleIds, CancellationToken cancellationToken = default)
        {
            await ResolveAuthorizationAsync(ModifyPolicy, cancellationToken);

            var group = await GroupRepository.Value.GetAsync(groupId, cancellationToken)
               ?? throw new NotFoundException($"Group not found");

            foreach (var roleId in roleIds)
            {
                if (!await ExistsAsync(roleId, groupId, cancellationToken))
                {
                    var role = await RoleRepository.Value.Get().FirstOrDefaultAsync(e => e.Id == roleId, cancellationToken)
                        ?? throw new NotFoundException($"Role not found");

                    Context.Entry(role).State = EntityState.Unchanged;
                    Context.Entry(group).State = EntityState.Unchanged;

                    await EntitySet.AddAsync(new Role_Group { Role = role, Group = group }, cancellationToken);

                    await Context.Logs.AddAsync(new Log { Type = Role.EntityType.Type, Action = LogAction.Link, Description = $"Added Group: {groupId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = roleId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                    await Context.Logs.AddAsync(new Log { Type = Group.EntityType.Type, Action = LogAction.Link, Description = $"Added Role: {roleId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = groupId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                }
            }

            await CommitAsync(cancellationToken);
            await ApplyPermissionsAsync(cancellationToken);
        }

        public async Task AddToGroups(Guid roleId, List<Guid> groupIds, CancellationToken cancellationToken = default)
        {
            await ResolveAuthorizationAsync(ModifyPolicy, cancellationToken);

            var role = await RoleRepository.Value.Get().FirstOrDefaultAsync(e => e.Id == roleId, cancellationToken)
                ?? throw new NotFoundException($"Role not found");

            foreach (var groupId in groupIds)
            {
                if (!await ExistsAsync(roleId, groupId, cancellationToken))
                {
                    var group = await GroupRepository.Value.GetAsync(groupId, cancellationToken)
                        ?? throw new NotFoundException($"Group not found");

                    Context.Entry(role).State = EntityState.Unchanged;
                    Context.Entry(group).State = EntityState.Unchanged;

                    await EntitySet.AddAsync(new Role_Group { Role = role, Group = group }, cancellationToken);

                    await Context.Logs.AddAsync(new Log { Type = Role.EntityType.Type, Action = LogAction.Link, Description = $"Added Group: {groupId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = roleId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                    await Context.Logs.AddAsync(new Log { Type = Group.EntityType.Type, Action = LogAction.Link, Description = $"Added Role: {roleId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = groupId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                }
            }

            await CommitAsync(cancellationToken);
            await ApplyPermissionsAsync(cancellationToken);
        }
    }
}
