using Microsoft.AspNetCore.OData.Deltas;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces
{
    public interface IBaseRepository<TEntity> : ICoreRepository<TEntity>
        where TEntity : class, IBaseModel
    {
        IQueryable<TEntity> GetRecycle();
        Task<TEntity> AddAsync(TEntity model, CancellationToken cancellationToken = default);

        Task<bool> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(TEntity model, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> GetRecycleAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> PatchAsync(Guid id, Delta<TEntity> model, CancellationToken cancellationToken = default);

        Task<bool> PurgeAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> RecoverAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity model, CancellationToken cancellationToken = default);

        IQueryable<ILog> GetLogs(Guid id);
    }
}
