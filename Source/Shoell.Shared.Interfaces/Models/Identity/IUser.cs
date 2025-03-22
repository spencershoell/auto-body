using Shoell.Shared.Interfaces.System;

namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUser : IBaseModel
    {
        string ExternalId { get; set; }

        ICollection<IUser_Group> UserGroups { get; set; }
        ICollection<ILog> Logs { get; set; }
        ICollection<IUser_Role> UserRoles { get; set; }
    }
}
