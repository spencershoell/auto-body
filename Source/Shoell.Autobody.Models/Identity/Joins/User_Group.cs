using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class User_Group : IUser_Group
    {
        public static EntityType EntityType => new(nameof(User_Group));

        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }

        public long RowId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;

        #region Implementation of IUser_Group
        IUser IUser_Group.User
        {
            get => User;
            set => User = value as User ?? new();
        }

        IGroup IUser_Group.Group
        {
            get => Group;
            set => Group = value as Group ?? new();
        }
        #endregion
    }
}
