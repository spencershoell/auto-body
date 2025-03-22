using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Deltas;
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
    public class UserRepository(
        IAuthorizationService authorizationService,
        IUserContext<User, Log> context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider
    ) : BaseRepository<User>
    {

        protected override EntityType EntityType => Models.Identity.User.EntityType;

        protected override IAuthorizationService AuthorizationService => authorizationService;
        protected override IUserContext<User, Log> Context => context;
        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected IServiceProvider ServiceProvider => serviceProvider;
        protected override DbSet<User> EntitySet => Context.Users;

        protected virtual Lazy<RoleRepository> RoleRepository => new(ServiceProvider.GetRequiredService<RoleRepository>());

        protected override string CreatePolicy => UserRoles.Create;
        protected override string ReadPolicy => UserRoles.Read;
        protected override string UpdatePolicy => UserRoles.Update;
        protected override string DeletePolicy => UserRoles.Delete;

        protected override string RecyclePolicy => UserRoles.Recycle;
        protected override string RecoverPolicy => UserRoles.Recover;
        protected override string PurgePolicy => UserRoles.Purge;

        protected override string ArchivePolicy => UserRoles.Archive;
        protected override string RestorePolicy => UserRoles.Restore;

        protected override Task<bool> ArchiveChildrenAsync(User model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public async override Task<bool> ExistsAsync(User model, CancellationToken cancellationToken = default)
        {
            return await Get()
                .AnyAsync(e => e.UserName == null
                    || e.UserName == model.UserName
                    || e.UserName == model.UserName
                    || e.UserName == (model.Email != null ? model.Email.ToUpper() : string.Empty), cancellationToken)
            || EntitySet.Local.Any(e => e.UserName == null
                    || e.UserName == model.UserName
                    || e.UserName == model.UserName
                    || e.UserName == (model.Email != null ? model.Email.ToUpper() : string.Empty))
            || await base.ExistsAsync(model, cancellationToken);
        }

        public override Task<User> AddAsync(User model, CancellationToken cancellationToken = default)
        {
            model.NormalizedUserName = model.UserName?.ToUpper();
            model.NormalizedEmail = model.Email?.ToUpper();
            return base.AddAsync(model, cancellationToken);
        }

        public async Task ApplyPermissionsAsync(CancellationToken cancellationToken = default)
        {
            await Context.ApplyPermissionsAsync(cancellationToken);
        }
        public User GetLoggedInUser()
        {
            var entity = Context.Users
                .FirstOrDefault(e => e.Id == User.AutobodyId())
               ?? throw new NotFoundException($"{EntityType.Name.AsSpacedPascaleCase()} not found");

            return entity;
        }

        public async Task<User> GetLoggedInUserAsync(CancellationToken cancellationToken = default)
        {
            var entity = await Context.Users
                .FirstOrDefaultAsync(e => e.Id == User.AutobodyId(), cancellationToken)
                ?? throw new NotFoundException($"{EntityType.Name.AsSpacedPascaleCase()} not found");

            return entity;
        }

        public IQueryable<Role> GetRoles()
        {
            var user = GetLoggedInUser()
                ?? throw new UserNotFoundException("User is not present");

            return GetRoles(user);
        }

        public IQueryable<Role> GetRoles(User user)
        {
            var roles = new List<Role>();

            return RoleRepository.Value.Get()
                .Where(e => e.UserRoles.Any(e => e.UserId == user.Id));
        }

        protected override Task<bool> PurgeChildrenAsync(User model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RestoreChildrenAsync(User model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RemoveChildrenAsync(User model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> RecoverChildrenAsync(User model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override void FilterUpdateProperties(Delta<User> delta, User model)
        {
            delta.TrySetPropertyValue(nameof(model.NormalizedUserName), model.NormalizedUserName);
            delta.TrySetPropertyValue(nameof(model.NormalizedEmail), model.NormalizedEmail);
            delta.TrySetPropertyValue(nameof(model.EmailConfirmed), model.EmailConfirmed);
            delta.TrySetPropertyValue(nameof(model.PasswordHash), model.PasswordHash);
            delta.TrySetPropertyValue(nameof(model.SecurityStamp), model.SecurityStamp);
            delta.TrySetPropertyValue(nameof(model.ConcurrencyStamp), model.ConcurrencyStamp);
            delta.TrySetPropertyValue(nameof(model.PhoneNumberConfirmed), model.PhoneNumberConfirmed);
            delta.TrySetPropertyValue(nameof(model.TwoFactorEnabled), model.TwoFactorEnabled);
            delta.TrySetPropertyValue(nameof(model.LockoutEnd), model.LockoutEnd);
            delta.TrySetPropertyValue(nameof(model.LockoutEnabled), model.LockoutEnabled);
            delta.TrySetPropertyValue(nameof(model.AccessFailedCount), model.AccessFailedCount);
        }
    }
}
