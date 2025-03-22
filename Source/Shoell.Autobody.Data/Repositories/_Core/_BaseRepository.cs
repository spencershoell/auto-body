using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Exceptions;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.System;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public abstract class BaseRepository<TEntity> : CoreRepository<TEntity>, IBaseRepository<TEntity>
        where TEntity : class, IBaseModel
    {
        protected abstract string CreatePolicy { get; }
        protected abstract string UpdatePolicy { get; }
        protected abstract string DeletePolicy { get; }

        protected abstract string RecyclePolicy { get; }
        protected abstract string RecoverPolicy { get; }
        protected abstract string PurgePolicy { get; }

        protected abstract string ArchivePolicy { get; }
        protected abstract string RestorePolicy { get; }

        public virtual IQueryable<TEntity> GetRecycle()
        {
            ResolveAuthorization(RecyclePolicy);
            return EntitySet
                .Where(e => e.DateDeleted != null)
                .IgnoreQueryFilters()
                .AsNoTracking();
        }
        public virtual async Task<TEntity> AddAsync(TEntity model, CancellationToken cancellationToken = default)
        {
            if (await ExistsAsync(model, cancellationToken))
                throw new BadRequestException($"Could not add {EntityType.Name.AsSpacedPascaleCase()}. Resource already exists.");

            await ResolveAuthorizationAsync(model, CreatePolicy, cancellationToken);

            // Attempting to commit requested resource to datastore
            model.CreatedById = User.AutobodyId();
            model.ModifiedById = User.AutobodyId();

            model.DateCreated = DateTimeProvider.UtcNow;
            model.DateModified = model.DateCreated;

            model.DateArchived = null;
            model.DateDeleted = null;
            model.ArchivedById = null;
            model.DeletedById = null;

            await EntitySet
                .AddAsync(model, cancellationToken);

            await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Create, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = model.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            return model;
        }

        public virtual async Task<bool> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await Get()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not archive {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, ArchivePolicy, cancellationToken);

            // Attempting to commit requested resource's removal from the datastore
            if (await ArchiveChildrenAsync(entity, cancellationToken))
            {
                // Updating requested resource's properties and modified information
                entity.ModifiedById = User.AutobodyId();
                entity.ArchivedById = User.AutobodyId();

                entity.DateModified = DateTimeProvider.UtcNow;
                entity.DateArchived = entity.DateModified;

                await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Archive, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
            }

            return true;
        }

        public virtual async Task<bool> ExistsAsync(TEntity model, CancellationToken cancellationToken = default)
        {
            return await Get()
                .AnyAsync(e =>
                       e.Id.Equals(model.Id)
                    || e.RowId.Equals(model.RowId), cancellationToken)
                || EntitySet.Local.Any(e => e.Id.Equals(model.Id) || e.RowId.Equals(model.RowId));
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Get()
                .AnyAsync(e =>
                       e.Id.Equals(id), cancellationToken)
                || EntitySet.Local.Any(e => e.Id.Equals(id));
        }

        public virtual async Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await Get()
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
                ?? throw new NotFoundException($"Could not find {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, ReadPolicy, cancellationToken);

            return entity;
        }

        public virtual async Task<TEntity> GetRecycleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource exists in the datastore
            var entity = await GetRecycle()
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
                ?? throw new NotFoundException($"Could not find deleted {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, RecyclePolicy, cancellationToken);

            return entity;
        }

        public virtual async Task<TEntity> PatchAsync(Guid id, Delta<TEntity> model, CancellationToken cancellationToken = default)
        {

            // Checking if the requested resource already exists in the datastore
            var entity = await Get()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not modify {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, UpdatePolicy, cancellationToken);

            model.TrySetPropertyValue(nameof(entity.Id), entity.Id);
            model.TrySetPropertyValue(nameof(entity.RowId), entity.RowId);
            model.TrySetPropertyValue(nameof(entity.IsArchived), entity.IsArchived);
            model.TrySetPropertyValue(nameof(entity.DateCreated), entity.DateCreated);
            model.TrySetPropertyValue(nameof(entity.DateModified), entity.DateModified);
            model.TrySetPropertyValue(nameof(entity.DateArchived), entity.DateArchived);
            model.TrySetPropertyValue(nameof(entity.DateDeleted), entity.DateDeleted);
            model.TrySetPropertyValue(nameof(entity.CreatedById), entity.CreatedById);
            model.TrySetPropertyValue(nameof(entity.ModifiedById), entity.ModifiedById);
            model.TrySetPropertyValue(nameof(entity.ArchivedById), entity.ArchivedById);
            model.TrySetPropertyValue(nameof(entity.DeletedById), entity.DeletedById);

            FilterUpdateProperties(model, entity);

            // Updating requested resource's properties and modified information
            model.Patch(entity);
            entity.DateModified = DateTimeProvider.UtcNow;
            entity.ModifiedById = User.AutobodyId();

            await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Update, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            return entity;
        }

        public virtual async Task<bool> PurgeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await GetRecycle()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not purge {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist in the recycle bin.");

            await ResolveAuthorizationAsync(entity, PurgePolicy, cancellationToken);

            // Attempting to commit requested resource's removal from the datastore
            if (await PurgeChildrenAsync(entity, cancellationToken))
                EntitySet.Remove(entity);

            await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Purge, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            return true;
        }

        public virtual async Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await Get()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not restore {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, RestorePolicy, cancellationToken);

            // Attempting to commit requested resource's removal from the datastore
            if (await RestoreChildrenAsync(entity, cancellationToken))
            {
                // Updating requested resource's properties and modified information
                entity.ModifiedById = User.AutobodyId();
                entity.ArchivedById = null;

                entity.DateModified = DateTimeProvider.UtcNow;
                entity.DateArchived = null;

                await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Restore, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
            }

            return true;
        }

        public virtual async Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await Get()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not Remove {EntityType.Name.AsSpacedPascaleCase()}. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, DeletePolicy, cancellationToken);

            // Attempting to commit requested resource's removal from the datastore
            if (await RemoveChildrenAsync(entity, cancellationToken))
            {
                // Updating requested resource's properties and modified information
                entity.ModifiedById = User.AutobodyId();
                entity.DeletedById = User.AutobodyId();

                entity.DateModified = DateTimeProvider.UtcNow;
                entity.DateDeleted = entity.DateModified;

                await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Delete, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);
            }

            return true;
        }

        public virtual async Task<TEntity> RecoverAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await GetRecycle()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Could not recover {EntityType.Name.AsSpacedPascaleCase()}. Resource has not been deleted.");

            await ResolveAuthorizationAsync(entity, RecoverPolicy, cancellationToken);

            // Updating requested resource's properties and modified information
            entity.ModifiedById = User.AutobodyId();
            entity.DeletedById = null;

            entity.DateModified = DateTimeProvider.UtcNow;
            entity.DateDeleted = null;

            // Attempting to commit requested resource's removal from the datastore
            await RecoverChildrenAsync(entity, cancellationToken);

            await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Recover, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = entity.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity model, CancellationToken cancellationToken = default)
        {
            // Checking if the requested resource already exists in the datastore
            var entity = await Get()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == model.Id, cancellationToken)
                ?? throw new NotFoundException($"Could not Update {EntityType.Name.AsSpacedPascaleCase()} in the datastore. Resource does not exist.");

            await ResolveAuthorizationAsync(entity, UpdatePolicy, cancellationToken);

            // Updating requested resource's properties and modified information
            model.CreatedById = entity.CreatedById;
            model.RowId = entity.RowId;
            model.ModifiedById = entity.ModifiedById;
            model.ArchivedById = entity.ArchivedById;
            model.DeletedById = entity.DeletedById;
            model.DateCreated = entity.DateCreated;
            model.DateModified = entity.DateModified;
            model.DateArchived = entity.DateArchived;
            model.DateDeleted = entity.DateDeleted;

            Context.Entry(entity).CurrentValues.SetValues(model);

            entity.ModifiedById = User.AutobodyId();
            entity.DateModified = DateTimeProvider.UtcNow;

            await Context.Logs.AddAsync(new Log { Type = EntityType.Type, Action = LogAction.Update, Description = string.Empty, IpAddress = $"{HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress}", UserAgent = $"{HttpContextAccessor.HttpContext?.Request.Headers.UserAgent}", UserId = User.AutobodyId(), EntityId = model.Id, DateCreated = DateTimeProvider.UtcNow }, cancellationToken);

            return entity;
        }

        public virtual IQueryable<Log> GetLogs(Guid id)
        {
            ResolveAuthorization(ReadPolicy);
            return Context.Logs.Where(e => e.EntityId == id && e.Type == EntityType.Type);
        }

        protected abstract void FilterUpdateProperties(Delta<TEntity> delta, TEntity model);

        protected abstract Task<bool> PurgeChildrenAsync(TEntity model, CancellationToken cancellationToken = default);
        protected abstract Task<bool> RemoveChildrenAsync(TEntity model, CancellationToken cancellationToken = default);
        protected abstract Task<bool> RecoverChildrenAsync(TEntity model, CancellationToken cancellationToken = default);
        protected abstract Task<bool> ArchiveChildrenAsync(TEntity model, CancellationToken cancellationToken = default);
        protected abstract Task<bool> RestoreChildrenAsync(TEntity model, CancellationToken cancellationToken = default);

        #region Implementation of IBaseRepository
        IQueryable<ILog> IBaseRepository<TEntity>.GetLogs(Guid id)
        {
            return GetLogs(id);
        }
        #endregion
    }
}
