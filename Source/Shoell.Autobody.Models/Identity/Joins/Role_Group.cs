using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class Role_Group : IRole_Group
    {
        public static EntityType EntityType => new(nameof(Role_Group));

        public Guid RoleId { get; set; }
        public Guid GroupId { get; set; }

        public long RowId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;

        #region Implementation of IRole_Group
        IRole IRole_Group.Role
        {
            get => Role;
            set => Role = value as Role ?? new();
        }

        IGroup IRole_Group.Group
        {
            get => Group;
            set => Group = value as Group ?? new();
        }
        #endregion
    }
}
