namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUser_Group
    {
        Guid UserId { get; set; }
        Guid GroupId { get; set; }

        long RowId { get; set; }

        IUser User { get; set; }
        IGroup Group { get; set; }
    }
}
