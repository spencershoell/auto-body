namespace Shoell.Shared.Interfaces.Identity
{
    public interface IGroup : IBaseModel
    {
        ICollection<IUser_Group> UserGroups { get; set; }
        ICollection<IRole_Group> RoleGroups { get; set; }
    }
}
