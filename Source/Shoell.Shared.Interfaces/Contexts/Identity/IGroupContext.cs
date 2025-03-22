using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IGroupContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IGroup
        where TLog : class, ILog
    {
        DbSet<TEntity> Groups { get; }
    }
}
