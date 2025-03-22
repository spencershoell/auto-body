namespace Shoell.Shared.Interfaces
{
    public interface IBaseJoinRepository<TEntity> : ICoreRepository<TEntity>
        where TEntity : class
    {
        Task<bool> ExistsAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(Guid id1, Guid id2, CancellationToken cancellationToken = default);
    }
}
