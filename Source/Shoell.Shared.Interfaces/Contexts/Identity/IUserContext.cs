using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUserContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IUser
        where TLog : class, ILog
    {
        DbSet<TEntity> Users { get; }
    }
}
