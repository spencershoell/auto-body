using Microsoft.AspNetCore.Identity;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class User_Role : IdentityUserRole<Guid>, IUser_Role
    {
        public static EntityType EntityType => new(nameof(User_Role));

        public long RowId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;

        #region Implementation of IUser_Role
        IUser IUser_Role.User
        {
            get => User;
            set => User = value as User ?? new();
        }

        IRole IUser_Role.Role
        {
            get => Role;
            set => Role = value as Role ?? new();
        }
        #endregion
    }
}
