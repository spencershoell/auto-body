using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IRoleContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IRole
        where TLog : class, ILog
    {
        DbSet<TEntity> Roles { get; }
    }
}
