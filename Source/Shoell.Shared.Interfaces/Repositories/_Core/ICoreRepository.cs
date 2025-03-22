namespace Shoell.Shared.Interfaces
{
    public interface ICoreRepository<TEntity>
        where TEntity : class
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        IQueryable<TEntity> Get();
    }
}
