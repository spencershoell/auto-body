using Shoell.Shared.Interfaces.Identity;

namespace Shoell.Shared.Interfaces
{
    public interface IBaseModel
    {
        Guid Id { get; set; }
        long RowId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool IsArchived { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        DateTime? DateArchived { get; set; }
        DateTime? DateDeleted { get; set; }

        Guid CreatedById { get; set; }
        Guid ModifiedById { get; set; }
        Guid? ArchivedById { get; set; }
        Guid? DeletedById { get; set; }

        IUser CreatedBy { get; set; }
        IUser ModifiedBy { get; set; }
        IUser? ArchivedBy { get; set; }
        IUser? DeletedBy { get; set; }
    }
}
