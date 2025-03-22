namespace Shoell.Shared.Interfaces.Identity
{
    public interface IRole_Group
    {
        Guid RoleId { get; set; }
        Guid GroupId { get; set; }

        long RowId { get; set; }

        IRole Role { get; set; }
        IGroup Group { get; set; }
    }
}
