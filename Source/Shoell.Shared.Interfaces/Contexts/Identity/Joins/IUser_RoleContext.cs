using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUser_RoleContext<TEntity, TLog> : ILogContext<TLog>
        where TEntity : class, IUser_Role
        where TLog : class, ILog
    {
        DbSet<TEntity> UserRoles { get; }
    }
}
