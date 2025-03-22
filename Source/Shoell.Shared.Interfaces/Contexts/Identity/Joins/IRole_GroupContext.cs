using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IRole_GroupContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IRole_Group
        where TLog : class, ILog
    {
        DbSet<TEntity> RoleGroups { get; }
    }
}
