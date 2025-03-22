using Microsoft.EntityFrameworkCore;

namespace Shoell.Shared.Interfaces.System
{
    public interface ILogContext<TEntity> : IDbContext
        where TEntity : class, ILog
    {
        DbSet<TEntity> Logs { get; }
    }
}
