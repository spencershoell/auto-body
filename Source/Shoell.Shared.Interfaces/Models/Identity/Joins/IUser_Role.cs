namespace Shoell.Shared.Interfaces.Identity
{
    public interface IUser_Role
    {
        Guid UserId { get; set; }
        Guid RoleId { get; set; }

        long RowId { get; set; }

        IUser User { get; set; }
        IRole Role { get; set; }
    }
}
