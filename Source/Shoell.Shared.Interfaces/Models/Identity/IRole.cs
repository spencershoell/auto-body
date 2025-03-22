namespace Shoell.Shared.Interfaces.Identity
{
    public interface IRole
    {
        long RowId { get; set; }
        string Target { get; set; }
        string Operation { get; set; }

        ICollection<IRole_Group> RoleGroups { get; set; }
        ICollection<IUser_Role> UserRoles { get; set; }
    }
}
