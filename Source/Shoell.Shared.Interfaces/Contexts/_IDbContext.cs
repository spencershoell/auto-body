using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shoell.Shared.Interfaces
{
    public interface IDbContext
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }

        Task ApplyPermissionsAsync(CancellationToken cancellationToken = default);

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class;

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
