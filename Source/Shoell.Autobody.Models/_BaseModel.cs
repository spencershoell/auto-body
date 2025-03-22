using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.Identity;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Models
{
    public abstract class BaseModel : IBaseModel
    {
        public static EntityType EntityType { get; } = new(nameof(BaseModel));

        public Guid Id { get; set; } = Guid.NewGuid();
        public long RowId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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

        #region Implementation of IBaseModel
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
    }
}
