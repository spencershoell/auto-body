using Microsoft.AspNetCore.Identity;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class Role : IdentityRole<Guid>, IRole
    {
        public static EntityType EntityType => new(nameof(Role));

        public long RowId { get; set; }
        public string Target { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;

        public virtual ICollection<Role_Group> RoleGroups { get; set; } = [];
        public virtual ICollection<User_Role> UserRoles { get; set; } = [];

        #region Implementation of IRole
        ICollection<IRole_Group> IRole.RoleGroups
        {
            get => RoleGroups as ICollection<IRole_Group> ?? [];
            set => RoleGroups = value as ICollection<Role_Group> ?? [];
        }

        ICollection<IUser_Role> IRole.UserRoles
        {
            get => UserRoles as ICollection<IUser_Role> ?? [];
            set => UserRoles = value as ICollection<User_Role> ?? [];
        }
        #endregion
    }
}
