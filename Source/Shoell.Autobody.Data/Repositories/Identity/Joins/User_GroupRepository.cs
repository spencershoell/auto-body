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
    public class User_GroupRepository(
        IAuthorizationService authorizationService,
        IUser_GroupContext<User_Group, Log> context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider
    ) : BaseJoinRepository<User_Group>
    {
        protected override EntityType EntityType => User_Group.EntityType;

        protected override EntityType EntityType1 => Models.Identity.User.EntityType;
        protected override EntityType EntityType2 => Group.EntityType;

        protected override IAuthorizationService AuthorizationService => authorizationService;
        protected override IUser_GroupContext<User_Group, Log> Context => context;
        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected IServiceProvider ServiceProvider => serviceProvider;
        protected override DbSet<User_Group> EntitySet => Context.UserGroups;

        protected virtual Lazy<GroupRepository> GroupRepository => new(ServiceProvider.GetRequiredService<GroupRepository>());
        protected virtual Lazy<UserRepository> UserRepository => new(ServiceProvider.GetRequiredService<UserRepository>());

        protected override string ModifyPolicy => User_GroupRoles.Modify;
        protected override string ReadPolicy => User_GroupRoles.Read;

        protected override Expression<Func<User_Group, bool>> IdentifierPredicate(Guid userId, Guid groupId)
        {
            return e => e.UserId == userId && e.GroupId == groupId;
        }

        public override async Task<bool> ExistsAsync(User_Group model, CancellationToken cancellationToken = default)
        {
            return await ExistsAsync(model.UserId, model.GroupId, cancellationToken);
        }

        public async Task AddToGroups(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken = default)
        {
            await ResolveAuthorizationAsync(ModifyPolicy, cancellationToken);

            var user = await UserRepository.Value.GetAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"User not found");

            foreach (var groupId in groupIds)
            {
                if (!await ExistsAsync(userId, groupId, cancellationToken))
                {
                    var group = await GroupRepository.Value.GetAsync(groupId, cancellationToken)
                        ?? throw new NotFoundException($"Group not found");

                    Context.Entry(user).State = EntityState.Unchanged;
                    Context.Entry(group).State = EntityState.Unchanged;

                    await EntitySet.AddAsync(new User_Group { User = user, Group = group }, cancellationToken);

                    await Context.Logs.AddAsync(new Log { Type = Group.EntityType.Type, Action = LogAction.Link, Description = $"Added User: {userId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = groupId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                    await Context.Logs.AddAsync(new Log { Type = Models.Identity.User.EntityType.Type, Action = LogAction.Link, Description = $"Added Group: {groupId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = userId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                }
            }

            await CommitAsync(cancellationToken);
            await ApplyPermissionsAsync(cancellationToken);
        }

        public async Task AddToUsers(Guid groupId, List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            await ResolveAuthorizationAsync(ModifyPolicy, cancellationToken);

            var group = await GroupRepository.Value.GetAsync(groupId, cancellationToken)
                ?? throw new NotFoundException($"Group not found");

            foreach (var userId in userIds)
            {
                if (!await ExistsAsync(userId, groupId, cancellationToken))
                {
                    var user = await UserRepository.Value.GetAsync(userId, cancellationToken)
                        ?? throw new NotFoundException($"User not found");

                    Context.Entry(user).State = EntityState.Unchanged;
                    Context.Entry(group).State = EntityState.Unchanged;

                    await EntitySet.AddAsync(new User_Group { User = user, Group = group }, cancellationToken);

                    await Context.Logs.AddAsync(new Log { Type = Group.EntityType.Type, Action = LogAction.Link, Description = $"Added User: {userId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = groupId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                    await Context.Logs.AddAsync(new Log { Type = Models.Identity.User.EntityType.Type, Action = LogAction.Link, Description = $"Added Group: {groupId}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = userId, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
                }
            }

            await CommitAsync(cancellationToken);
            await ApplyPermissionsAsync(cancellationToken);
        }
    }
}
