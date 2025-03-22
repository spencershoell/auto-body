using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Interfaces.System;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.System
{
    public class Log : ILog
    {
        public static EntityType EntityType => new(nameof(Log));
        public Guid Id { get; set; } = Guid.NewGuid();
        public long RowId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        #region Implementation of IRepositoryEntity
        IUser ILog.User
        {
            get => User;
            set => User = value as User ?? new();
        }
        #endregion
    }
}
