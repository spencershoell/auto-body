using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUser_GroupContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IUser_Group
        where TLog : class, ILog
    {
        DbSet<TEntity> UserGroups { get; }
    }
}
