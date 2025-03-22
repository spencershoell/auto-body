using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class Group : BaseModel, IGroup
    {
        public static new EntityType EntityType => new(nameof(Group));

        public virtual ICollection<User_Group> UserGroups { get; set; } = [];
        public virtual ICollection<Role_Group> RoleGroups { get; set; } = [];

        #region Implementation of IGroup
        ICollection<IUser_Group> IGroup.UserGroups
        {
            get => UserGroups as ICollection<IUser_Group> ?? [];
            set => UserGroups = value as ICollection<User_Group> ?? [];
        }
        ICollection<IRole_Group> IGroup.RoleGroups
        {
            get => RoleGroups as ICollection<IRole_Group> ?? [];
            set => RoleGroups = value as ICollection<Role_Group> ?? [];
        }
        #endregion
    }
}
