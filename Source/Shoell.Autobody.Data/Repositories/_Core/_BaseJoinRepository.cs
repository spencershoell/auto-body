using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Exceptions;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public abstract class BaseJoinRepository<TEntity> : CoreRepository<TEntity>, IBaseJoinRepository<TEntity>
        where TEntity : class
    {
        protected abstract string ModifyPolicy { get; }

        protected abstract EntityType EntityType1 { get; }
        protected abstract EntityType EntityType2 { get; }

        protected abstract Expression<Func<TEntity, bool>> IdentifierPredicate(Guid id1, Guid id2);

        public abstract Task<bool> ExistsAsync(TEntity model, CancellationToken cancellationToken = default);

        public virtual async Task<bool> ExistsAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default)
        {
            return await Get().AnyAsync(IdentifierPredicate(id1, id2), cancellationToken);
        }

        public virtual async Task<TEntity> GetAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default)
        {
            var entity = await Get()
                .FirstOrDefaultAsync(IdentifierPredicate(id1, id2), cancellationToken)
                ?? throw new NotFoundException($"Could not find {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, ReadPolicy, cancellationToken);
            return entity;
        }

        public async Task<bool> RemoveAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id1, id2, cancellationToken)
               ?? throw new NotFoundException($"Could not Remove {EntityType.Name.AsSpacedPascaleCase()} from the datastore. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, ModifyPolicy, cancellationToken);

            EntitySet.Attach(entity);
            EntitySet.Remove(entity);

            await Context.Logs.AddAsync(new Log { Type = EntityType1.Type, Action = LogAction.Link, Description = $"Removed Group: {id2}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = id1, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
            await Context.Logs.AddAsync(new Log { Type = EntityType2.Type, Action = LogAction.Link, Description = $"Removed Role: {id1}", IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = id2, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            await CommitAsync(cancellationToken);
            await ApplyPermissionsAsync(cancellationToken);

            return true;
        }

        protected async Task ApplyPermissionsAsync(CancellationToken cancellationToken = default)
        {
            await Context.ApplyPermissionsAsync(cancellationToken);
        }
    }
}
