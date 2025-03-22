using Shoell.Shared.Interfaces.Identity;

namespace Shoell.Shared.Interfaces.System
{
    public interface ILog
    {
        Guid Id { get; set; }
        long RowId { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        string Action { get; set; }
        string Description { get; set; }
        string IpAddress { get; set; }
        string UserAgent { get; set; }
        DateTime DateCreated { get; set; }

        Guid EntityId { get; set; }
        Guid UserId { get; set; }
        IUser User { get; set; }
    }
}
