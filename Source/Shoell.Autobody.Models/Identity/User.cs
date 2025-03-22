using Microsoft.AspNetCore.Identity;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Interfaces.System;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models.Identity
{
    public class User : IdentityUser<Guid>, IUser
    {
        public static EntityType EntityType => new(nameof(User));
        public long RowId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public bool IsArchived { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime? DateArchived { get; set; }
        public DateTime? DateDeleted { get; set; }

        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid? ArchivedById { get; set; }
        public Guid? DeletedById { get; set; }

        public User CreatedBy { get; set; } = null!;
        public User ModifiedBy { get; set; } = null!;
        public User? ArchivedBy { get; set; }
        public User? DeletedBy { get; set; }

        public virtual ICollection<User_Group> UserGroups { get; set; } = [];
        public virtual ICollection<Log> Logs { get; set; } = [];
        public virtual ICollection<User_Role> UserRoles { get; set; } = [];

        public User()
        {
            Id = Guid.NewGuid();
            UserName = string.Empty;
            NormalizedUserName = string.Empty;
            Email = string.Empty;
            NormalizedEmail = string.Empty;
            EmailConfirmed = false;
            PasswordHash = string.Empty;
            SecurityStamp = Guid.NewGuid().ToString();
            PhoneNumber = string.Empty;
            PhoneNumberConfirmed = false;
            TwoFactorEnabled = false;
            LockoutEnd = null;
            LockoutEnabled = false;
            AccessFailedCount = 0;
        }

        #region Implementation of IRepositoryEntity
        IUser IBaseModel.CreatedBy
        {
            get => CreatedBy;
            set => CreatedBy = value as User ?? new();
        }

        IUser IBaseModel.ModifiedBy
        {
            get => ModifiedBy;
            set => ModifiedBy = value as User ?? new();
        }

        IUser? IBaseModel.ArchivedBy
        {
            get => ArchivedBy;
            set => ArchivedBy = value as User;
        }

        IUser? IBaseModel.DeletedBy
        {
            get => DeletedBy;
            set => DeletedBy = value as User;
        }
        #endregion

        #region Implementation of IUser
        ICollection<IUser_Group> IUser.UserGroups
        {
            get => UserGroups as ICollection<IUser_Group> ?? [];
            set => UserGroups = value as ICollection<User_Group> ?? [];
        }

        ICollection<ILog> IUser.Logs
        {
            get => Logs as ICollection<ILog> ?? [];
            set => Logs = value as ICollection<Log> ?? [];
        }

        ICollection<IUser_Role> IUser.UserRoles
        {
            get => UserRoles as ICollection<IUser_Role> ?? [];
            set => UserRoles = value as ICollection<User_Role> ?? [];
        }
        #endregion
    }
}
